using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.API.Data;
using ServiceDesk.API.DTOs;
using ServiceDesk.API.DTOs.Categories;
using ServiceDesk.API.DTOs.Tickets;
using ServiceDesk.API.Models;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(AppDbContext db, ILogger<TicketsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Create a new ticket. Student only.</summary>
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TicketResponse>> Create([FromBody] CreateTicketRequest request)
    {
        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["title"] = ["Title must not be empty or whitespace."] }
            });
        }

        if (!Enum.TryParse<TicketPriority>(request.Priority, ignoreCase: true, out var priority))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["priority"] = [$"Invalid priority. Allowed values: {string.Join(", ", Enum.GetNames<TicketPriority>())}."] }
            });
        }

        var category = await _db.Categories.FindAsync(request.CategoryId);
        if (category is null || !category.IsActive)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: category is null
                    ? $"Category {request.CategoryId} does not exist."
                    : $"Category {request.CategoryId} is inactive and cannot be used.");
        }

        var userId = User.FindFirstValue("sub")!;
        var now = DateTimeOffset.UtcNow;

        var ticket = new Ticket
        {
            Title = title,
            Description = request.Description,
            Priority = priority,
            Status = TicketStatus.New,
            AuthorId = userId,
            AssigneeId = null,
            RejectedReason = null,
            CreatedAt = now,
            UpdatedAt = now,
            CategoryId = request.CategoryId
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        var created = await TicketQuery().FirstAsync(t => t.Id == ticket.Id);

        _logger.LogInformation("Ticket {TicketId} created by user {UserId}", ticket.Id, userId);

        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, MapToResponse(created));
    }

    /// <summary>
    /// List tickets with optional filters and pagination.
    /// Students always see only their own tickets; assignedToMe is ignored for Students.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<TicketResponse>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        [FromQuery] bool assignedToMe = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var role = User.FindFirstValue("role") ?? string.Empty;
        var userId = User.FindFirstValue("sub")!;

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = TicketQuery();

        if (role == "Student")
        {
            // Server always enforces own-tickets only; ignore assignedToMe param
            if (assignedToMe)
                _logger.LogWarning("Student {UserId} passed assignedToMe=true — param ignored", userId);

            query = query.Where(t => t.AuthorId == userId);
        }
        else if (assignedToMe)
        {
            query = query.Where(t => t.AssigneeId == userId);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TicketStatus>(status, ignoreCase: true, out var statusEnum))
            query = query.Where(t => t.Status == statusEnum);

        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<TicketPriority>(priority, ignoreCase: true, out var priorityEnum))
            query = query.Where(t => t.Priority == priorityEnum);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResponse<TicketResponse>(
            items.Select(MapToResponse),
            page,
            pageSize,
            total));
    }

    /// <summary>
    /// Get a single ticket by id.
    /// Students can only access their own ticket — foreign ticket returns 404.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> GetById(int id)
    {
        var role = User.FindFirstValue("role") ?? string.Empty;
        var userId = User.FindFirstValue("sub")!;

        var ticket = await TicketQuery().FirstOrDefaultAsync(t => t.Id == id);

        if (ticket is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"Ticket {id} not found.");

        // Student sees foreign ticket as 404 (not 403)
        if (role == "Student" && ticket.AuthorId != userId)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"Ticket {id} not found.");

        return Ok(MapToResponse(ticket));
    }

    private IQueryable<Ticket> TicketQuery() =>
        _db.Tickets
            .Include(t => t.Author)
            .Include(t => t.Assignee)
            .Include(t => t.Category)
            .AsNoTracking();

    private static TicketResponse MapToResponse(Ticket t) => new(
        t.Id,
        t.Title,
        t.Description,
        t.Priority.ToString(),
        t.Status.ToString(),
        t.CreatedAt,
        t.UpdatedAt,
        t.RejectedReason,
        new CategoryResponse(t.Category.Id, t.Category.Name, t.Category.IsActive),
        new UserBriefResponse(t.Author.Id, t.Author.DisplayName),
        t.Assignee is not null ? new UserBriefResponse(t.Assignee.Id, t.Assignee.DisplayName) : null
    );
}
