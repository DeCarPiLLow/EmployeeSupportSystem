namespace EmployeeSupportSystem.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
    }

    public enum TicketStatus
    {
        Pending,
        Allocated,
        Active,
        Resolved
    }
}
