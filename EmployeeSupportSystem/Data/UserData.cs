using System.Collections.Generic;
using System.Linq;
using EmployeeSupportSystem.Models;
using EmployeeSupportSystem.Controllers;
using System.Text.RegularExpressions;

namespace EmployeeSupportSystem.Data
{
    public static class UserData
    {
        private static List<User> Users = new List<User>
        {
            new User { Id = "1", Username = "admin", Password = "admin123", UserRole = Role.Admin },
            new User { Id = "2", Username = "agent", Password = "agent1", UserRole = Role.SupportAgent },
            new User { Id = "3", Username = "emp1", Password = "emp1", UserRole = Role.Employee },
            new User { Id = "4", Username = "emp2", Password = "emp2", UserRole = Role.Employee }
        };

        public static User ValidateUser(string id, string password)
        {
            return Users.FirstOrDefault(u => u.Id == id && u.Password == password);
        }

        public static bool CreateUser(string id, string username, string password, out string errorMessage)
        {
            if (Users.Any(user => user.Id == id))
            {
                errorMessage = "Id already exists";
                return false; 
            }

            if (!IsValidPassword(password))
            {
                errorMessage = "Password must be 8-20 character length, should have atleast 1 character each in uppercase, lowercase, number and special characters!!";
            }

            Users.Add(new User { Id = id, Username = username, Password = password, UserRole = Role.Employee });
            errorMessage = string.Empty;
            return true;
        }

        public static List<User> GetAllUsers()
        {
            return Users;
        }

        private static bool IsValidPassword(string password)
        {
            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}");
            return passwordPattern.IsMatch(password);
        }
    }
}
