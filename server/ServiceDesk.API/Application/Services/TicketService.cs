using Microsoft.EntityFrameworkCore;
using ServiceDesk.API.Application.Mapping;
using ServiceDesk.API.Domain;
using ServiceDesk.API.DTOs;
using ServiceDesk.API.DTOs.Tickets;
using ServiceDesk.API.Exceptions;
using ServiceDesk.API.Infrastructure.Data;

namespace ServiceDesk.API.Application.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TicketService> _logger;

    public TicketService(AppDbContext db, ILogger<TicketService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResponse<TicketResponse>> GetAllAsync(TicketsQuery query, string userId, string role)
    {
        var page     = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = BaseQuery();

        if (role == "Student")
        {
            if (query.AssignedToMe)
                _logger.LogWarning("Student {UserId} passed assignedToMe=true — param ignored", userId);
            q = q.Where(t => t.AuthorId == userId);
        }
        else if (query.AssignedToMe)
        {
            q = q.Where(t => t.AssigneeId == userId);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<TicketStatus>(query.Status, ignoreCase: true, out var statusEnum))
            q = q.Where(t => t.Status == statusEnum);

        if (!string.IsNullOrWhiteSpace(query.Priority) &&
            Enum.TryParse<TicketPriority>(query.Priority, ignoreCase: true, out var priorityEnum))
            q = q.Where(t => t.Priority == priorityEnum);

        if (query.CategoryId.HasValue)
            q = q.Where(t => t.CategoryId == query.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(t => t.Title.ToLower().Contains(query.Search.ToLower()));

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<TicketResponse>(
            items.Select(t => t.ToResponse()),
            page,
            pageSize,
            total);
    }

    public async Task<TicketResponse> GetByIdAsync(int id, string userId, string role)
    {
        var ticket = await BaseQuery().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException($"Ticket {id} not found.");

        // Student sees foreign ticket as 404 (not 403)
        if (role == "Student" && ticket.AuthorId != userId)
            throw new NotFoundException($"Ticket {id} not found.");

        return ticket.ToResponse();
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, string userId, string role)
    {
        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessException("Title must not be empty or whitespace.");

        if (!Enum.TryParse<TicketPriority>(request.Priority, ignoreCase: true, out var priority))
            throw new BusinessException(
                $"Invalid priority. Allowed values: {string.Join(", ", Enum.GetNames<TicketPriority>())}.");

        var category = await _db.Categories.FindAsync(request.CategoryId);
        if (category is null)
            throw new BusinessException($"Category {request.CategoryId} does not exist.");
        if (!category.IsActive)
            throw new BusinessException($"Category {request.CategoryId} is inactive and cannot be used.");

        var now = DateTimeOffset.UtcNow;
        var ticket = new Ticket
        {
            Title          = title,
            Description    = request.Description,
            Priority       = priority,
            Status         = TicketStatus.New,
            AuthorId       = userId,
            AssigneeId     = null,
            RejectedReason = null,
            CreatedAt      = now,
            UpdatedAt      = now,
            CategoryId     = request.CategoryId
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        var created = await BaseQuery().FirstAsync(t => t.Id == ticket.Id);

        _logger.LogInformation("Ticket {TicketId} created by user {UserId}", ticket.Id, userId);

        return created.ToResponse();
    }

    public async Task<TicketResponse> AssignToMeAsync(int id, string currentOperatorId)
    {
        // 1. Ticket exists
        var ticket = await TrackedQuery().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException($"Ticket {id} not found.");

        // 2. Status must be New, InProgress, or WaitingForStudent
        var assignableStatuses = new[] { TicketStatus.New, TicketStatus.InProgress, TicketStatus.WaitingForStudent };
        if (!assignableStatuses.Contains(ticket.Status))
            throw new BusinessException(
                $"Ticket {id} cannot be assigned in status '{ticket.Status}'. " +
                $"Allowed statuses: {string.Join(", ", assignableStatuses)}.");

        // 3. Anti-steal check
        if (ticket.AssigneeId is not null && ticket.AssigneeId != currentOperatorId)
            throw new ConflictException(
                $"Ticket {id} is already assigned to another operator.");

        // 4. Assign and promote status
        ticket.AssigneeId = currentOperatorId;
        if (ticket.Status == TicketStatus.New)
            ticket.Status = TicketStatus.InProgress;

        // 5. Timestamp
        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        var updated = await BaseQuery().FirstAsync(t => t.Id == id);
        _logger.LogInformation("Ticket {TicketId} assigned to operator {OperatorId}", id, currentOperatorId);
        return updated.ToResponse();
    }

    public async Task<TicketResponse> ChangeStatusAsync(int id, string currentOperatorId, string newStatus)
    {
        // 1. Ticket exists
        var ticket = await TrackedQuery().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException($"Ticket {id} not found.");

        // 2. Must be the assignee
        if (ticket.AssigneeId != currentOperatorId)
            throw new ForbiddenException("Only the assigned operator can change the ticket status.");

        // 3. Parse and check same status
        if (!Enum.TryParse<TicketStatus>(newStatus, ignoreCase: true, out var targetStatus))
            throw new BusinessException(
                $"Invalid status value '{newStatus}'. " +
                $"Allowed values: {string.Join(", ", Enum.GetNames<TicketStatus>())}.");

        if (ticket.Status == targetStatus)
            throw new BusinessException($"Ticket {id} is already in status '{ticket.Status}'.");

        // 4. Validate allowed workflow transitions
        var allowedTransitions = new Dictionary<TicketStatus, TicketStatus[]>
        {
            [TicketStatus.InProgress]        = [TicketStatus.WaitingForStudent, TicketStatus.Resolved],
            [TicketStatus.WaitingForStudent] = [TicketStatus.InProgress, TicketStatus.Resolved],
        };

        if (!allowedTransitions.TryGetValue(ticket.Status, out var allowed) || !allowed.Contains(targetStatus))
            throw new BusinessException(
                $"Transition from '{ticket.Status}' to '{targetStatus}' is not allowed via this endpoint.");

        // 5. Apply and timestamp
        ticket.Status    = targetStatus;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        var updated = await BaseQuery().FirstAsync(t => t.Id == id);
        _logger.LogInformation("Ticket {TicketId} status changed to {Status} by operator {OperatorId}",
            id, targetStatus, currentOperatorId);
        return updated.ToResponse();
    }

    public async Task<TicketResponse> RejectAsync(int id, string currentOperatorId, string reason)
    {
        // 1. Ticket exists
        var ticket = await TrackedQuery().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException($"Ticket {id} not found.");

        // 2. Closed/Rejected tickets cannot be rejected again
        if (ticket.Status is TicketStatus.Closed or TicketStatus.Rejected)
            throw new BusinessException(
                $"Ticket {id} is already in status '{ticket.Status}' and cannot be rejected.");

        // 3. Anti-steal check
        if (ticket.AssigneeId is not null && ticket.AssigneeId != currentOperatorId)
            throw new ConflictException(
                $"Ticket {id} is already assigned to another operator.");

        // 4. Auto-assign if New with no assignee
        if (ticket.Status == TicketStatus.New && ticket.AssigneeId is null)
            ticket.AssigneeId = currentOperatorId;

        // 5. Reject
        ticket.Status         = TicketStatus.Rejected;
        ticket.RejectedReason = reason;
        ticket.UpdatedAt      = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        var updated = await BaseQuery().FirstAsync(t => t.Id == id);
        _logger.LogInformation("Ticket {TicketId} rejected by operator {OperatorId} with reason: {Reason}",
            id, currentOperatorId, reason);
        return updated.ToResponse();
    }

    private IQueryable<Ticket> BaseQuery() =>
        _db.Tickets
            .Include(t => t.Author)
            .Include(t => t.Assignee)
            .Include(t => t.Category)
            .AsNoTracking();

    private IQueryable<Ticket> TrackedQuery() =>
        _db.Tickets
            .Include(t => t.Author)
            .Include(t => t.Assignee)
            .Include(t => t.Category);
}
