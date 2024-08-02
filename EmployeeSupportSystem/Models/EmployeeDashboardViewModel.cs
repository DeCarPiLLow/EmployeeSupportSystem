namespace EmployeeSupportSystem.Models
{
    public class EmployeeDashboardViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public NewTicketViewModel NewTicket { get; set; }
    }

    public class NewTicketViewModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
