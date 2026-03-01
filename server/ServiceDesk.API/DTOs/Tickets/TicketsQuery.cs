namespace ServiceDesk.API.DTOs.Tickets;

public record TicketsQuery(
    string? Status,
    string? Priority,
    int? CategoryId,
    string? Search,
    bool AssignedToMe,
    int Page,
    int PageSize
);
