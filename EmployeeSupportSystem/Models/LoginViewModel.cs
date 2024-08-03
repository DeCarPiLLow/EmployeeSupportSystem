using System.ComponentModel.DataAnnotations;

namespace EmployeeSupportSystem.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
