using ServiceDesk.API.Domain;
using ServiceDesk.API.DTOs.Categories;
using ServiceDesk.API.DTOs.Tickets;

namespace ServiceDesk.API.Application.Mapping;

public static class TicketMapping
{
    public static TicketResponse ToResponse(this Ticket t) => new(
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
