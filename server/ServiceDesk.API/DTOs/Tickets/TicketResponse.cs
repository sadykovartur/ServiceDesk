using ServiceDesk.API.DTOs.Categories;

namespace ServiceDesk.API.DTOs.Tickets;

public record TicketResponse(
    int Id,
    string Title,
    string Description,
    string Priority,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? RejectedReason,
    CategoryResponse Category,
    UserBriefResponse Author,
    UserBriefResponse? Assignee
);
