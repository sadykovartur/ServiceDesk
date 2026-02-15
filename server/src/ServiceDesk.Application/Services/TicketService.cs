using ServiceDesk.Application.DTOs;
using ServiceDesk.Application.Interfaces.Repositories;
using ServiceDesk.Application.Interfaces.Services;
using ServiceDesk.Domain.Entities;
using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Application.Services;

public class TicketService(
    ITicketRepository ticketRepository,
    ICategoryRepository categoryRepository) : ITicketService
{
    public async Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await ticketRepository.GetAllAsync(cancellationToken);
        return tickets.Select(MapToDto).ToList();
    }

    public async Task<TicketDto?> GetByIdAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<TicketDto> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default)
    {
        var categoryExists = await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new KeyNotFoundException($"Category with id {request.CategoryId} was not found.");
        }

        var now = DateTimeOffset.UtcNow;
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CategoryId = request.CategoryId,
            Priority = request.Priority,
            Status = TicketStatus.New,
            AuthorId = request.AuthorId.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await ticketRepository.CreateAsync(ticket, cancellationToken);
        var loaded = await ticketRepository.GetByIdAsync(created.Id, cancellationToken) ?? created;
        return MapToDto(loaded);
    }

    private static TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.CategoryId,
            ticket.Category?.Name ?? string.Empty,
            ticket.Priority,
            ticket.Status,
            ticket.AuthorId,
            ticket.AssigneeId,
            ticket.RejectedReason,
            ticket.CreatedAt,
            ticket.UpdatedAt
        );
    }
}
