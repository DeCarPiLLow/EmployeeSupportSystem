namespace EmployeeSupportSystem.Models
{
    public class SupportAgentViewModel
    {
        public List<Ticket> ResolvedTickets { get; set; }
        public List<Ticket> PendingTickets { get; set; }
    }
}
