namespace ServiceDesk.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Ticket? Ticket { get; set; }
}
