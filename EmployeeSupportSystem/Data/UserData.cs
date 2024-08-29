using EmployeeSupportSystem.Models; // Importing models from the EmployeeSupportSystem namespace
using System.Text.RegularExpressions; // Using regular expressions for password validation

namespace EmployeeSupportSystem.Data
{
    public class UserData
    {
        private readonly DataContext _context;

        public UserData(DataContext context)
        {
            _context = context;
        }

        public User ValidateUser(string id, string password)
        {
            // Validates user credentials by checking ID and password
            return _context.Users.FirstOrDefault(u => u.Id == id && u.Password == password);
        }

        public bool CreateUser(string id, string username, string password, out string errorMessage)
        {
            if (_context.Users.Any(user => user.Id == id))
            {
                errorMessage = "Id already exists"; // Error if the ID is already taken
                return false;
            }

            if (!IsValidPassword(password))
            {
                errorMessage = "Password must be 8-20 characters long, and include at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character."; // Error if password does not meet criteria
                return false;
            }

            // Add new user to the list
            _context.Users.Add(new User { Id = id, Username = username, Password = password, UserRole = Role.Employee });
            _context.SaveChanges();

            errorMessage = string.Empty;
            return true;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList(); // Returns the list of all users
        }

        private static bool IsValidPassword(string password)
        {
            // Regular expression to validate password strength
            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}");
            return passwordPattern.IsMatch(password);
        }
    }
}
