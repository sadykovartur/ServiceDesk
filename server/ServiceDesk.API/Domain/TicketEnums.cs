namespace ServiceDesk.API.Domain;

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
