using System.ComponentModel.DataAnnotations;

namespace EmployeeSupportSystem.Models
{
    public class SignupViewModel
    {
        [Required]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 4)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength (20, MinimumLength = 8)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password doesn't match!!")]
        public string ConfirmPassword { get; set; }
    }
}
