using EmployeeSupportSystem.Models;
using System.Text.RegularExpressions;

namespace EmployeeSupportSystem.Data
{
    public class TicketData
    {
        private static List<Ticket> Tickets = new List<Ticket>();

        public static bool CreateNewicket(string userId, string subject, string description)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid().ToString(),
                Subject = subject,
                Description = description,
                Status = Status.Pending,
                UserId = userId
            };
            Tickets.Add(ticket);
            return true;
        }

        public static List<Ticket> GetAllTickets()
        {
            return Tickets; 
        }
    }
}
