using System.ComponentModel.DataAnnotations;

namespace EmployeeSupportSystem.Models
{
    public class CreateNewTicketViewModel
    {
        [Required]
        [Display (Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display (Name = "Description")]
        public string Description { get; set; }
    }
}
