namespace ServiceDesk.API.Models;

public enum TicketStatus
{
    New,
    InProgress,
    WaitingForStudent,
    Resolved,
    Closed,
    Rejected
}

public enum TicketPriority
{
    Low,
    Medium,
    High
}
