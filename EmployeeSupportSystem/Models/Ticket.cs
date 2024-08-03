namespace EmployeeSupportSystem.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string UserId { get; set; }
        public string AgentId { get; set; }
    }

    public enum Status
    {
        Pending,
        Assigned,
        Active,
        Resolved
    }
}
