namespace EmployeeSupportSystem.Models
{
    public enum Role
    {
        Admin,
        SupportAgent,
        Employee
    }
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role UserRole { get; set; }
    }
}
