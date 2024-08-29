using System.ComponentModel.DataAnnotations;

public enum TicketStatus
{
    //Pending,
    Assigned,
    InProgress,
    //Resolved,
    Pending,
    Allocated, 
    Active,
    Resolved
}

public class Ticket
{
    [Key]
    public string TicketID { get; set; }
    public string CreatedBy { get; set; }
    public string? AssignedTo { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AssignedAt { get; set; } // Timestamp for when the ticket is assigned
    public DateTime? ActiveAt { get; set; }   // Timestamp for when the ticket becomes active
    public DateTime? ResolvedAt { get; set; } // Timestamp for when the ticket is resolved
}
