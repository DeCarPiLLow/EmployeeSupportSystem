using System.ComponentModel.DataAnnotations;

namespace EmployeeSupportSystem.Models
{
    public class TicketViewModel
    {
        [Required]
        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
    }
}
