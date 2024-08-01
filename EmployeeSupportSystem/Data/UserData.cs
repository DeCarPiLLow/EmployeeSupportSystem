using System.Collections.Generic;
using System.Linq;
using EmployeeSupportSystem.Models;

namespace EmployeeSupportSystem.Data
{
    public static class UserData
    {
        public static List<User> Users = new List<User>
        {
            new User { Id = "1", Username = "admin", Password = "admin123", UserRole = Role.Admin },
            new User { Id = "2", Username = "agent", Password = "agent1", UserRole = Role.SupportAgent },
            new User { Id = "3", Username = "emp1", Password = "emp1", UserRole = Role.Empolyee },
            new User { Id = "4", Username = "emp2", Password = "emp2", UserRole = Role.Empolyee }
        };

        public static User ValidateUser(string username, string password)
        {
            return Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
