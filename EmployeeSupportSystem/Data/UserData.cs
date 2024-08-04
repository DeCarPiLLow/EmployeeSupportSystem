using System.Collections.Generic; // Using collections functionality
using System.Linq; // Using LINQ functionality
using EmployeeSupportSystem.Models; // Importing models from the EmployeeSupportSystem namespace
using EmployeeSupportSystem.Controllers; // Importing controllers from the EmployeeSupportSystem namespace (not used in this snippet)
using System.Text.RegularExpressions; // Using regular expressions for password validation

namespace EmployeeSupportSystem.Data
{
    public static class UserData
    {
        // Static list of users for in-memory storage
        public static List<User> Users = new List<User>
        {
            new User { Id = "1", Username = "admin", Password = "admin123", UserRole = Role.Admin },
            new User { Id = "2", Username = "agent", Password = "agent1", UserRole = Role.SupportAgent },
            new User { Id = "3", Username = "emp1", Password = "emp1", UserRole = Role.Employee },
            new User { Id = "4", Username = "emp2", Password = "emp2", UserRole = Role.Employee }
        };

        public static User ValidateUser(string id, string password)
        {
            // Validates user credentials by checking ID and password
            return Users.FirstOrDefault(u => u.Id == id && u.Password == password);
        }

        public static bool CreateUser(string id, string username, string password, out string errorMessage)
        {
            if (Users.Any(user => user.Id == id))
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
            Users.Add(new User { Id = id, Username = username, Password = password, UserRole = Role.Employee });
            errorMessage = string.Empty;
            return true;
        }

        public static List<User> GetAllUsers()
        {
            return Users; // Returns the list of all users
        }

        private static bool IsValidPassword(string password)
        {
            // Regular expression to validate password strength
            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}");
            return passwordPattern.IsMatch(password);
        }
    }
}
