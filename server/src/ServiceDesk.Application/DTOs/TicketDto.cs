using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Application.DTOs;

public record TicketDto(
    int Id,
    string Title,
    string Description,
    int CategoryId,
    string CategoryName,
    Priority Priority,
    TicketStatus Status,
    string AuthorId,
    string? AssigneeId,
    string? RejectedReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
