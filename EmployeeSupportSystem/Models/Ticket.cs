using System;

public enum TicketStatus
{
    Pending,
    Assigned,
    InProgress,
    Resolved
}

public class Ticket
{
    public string TicketID { get; set; }
    public string CreatedBy { get; set; }
    public string AssignedTo { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}