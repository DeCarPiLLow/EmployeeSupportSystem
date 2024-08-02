using System.ComponentModel.DataAnnotations;

namespace EmployeeSupportSystem.Models
{
    public class TicketViewModel
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
    }

    public class CreateTicketViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Description { get; set; }
    }
}

