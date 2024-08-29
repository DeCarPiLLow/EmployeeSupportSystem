namespace EmployeeSupportSystem.Models
{
    public class TicketAnalyticsViewModel
    {
        public string TicketID { get; set; }
        public double TimePending { get; set; }
        public double TimeAllocated { get; set; }
        public double TimeActive { get; set; }
        public double TimeResolved { get; set; }
    }
}
