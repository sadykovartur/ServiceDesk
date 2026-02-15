using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public string AuthorId { get; set; } = string.Empty;
    public string? AssigneeId { get; set; }
    public string? RejectedReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Category? Category { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
