namespace EmployeeSupportSystem.Data
{
    public class TicketData
    { 
        private static List<Ticket> Tickets = new List<Ticket>();

        public static List<Ticket> GetAllTickets()
        {
            return Tickets;
        }

        public static List<Ticket> GetTicketsByCreator(string creator)
        {
            return Tickets.Where(ticket => ticket.CreatedBy == creator).ToList();
        }

        public static List<Ticket> GetTicketsByAssignee(string assignee)
        {
            return Tickets.Where(ticket => ticket.AssignedTo == assignee).ToList();
        }

        public static void AddTicket(Ticket ticket)
        {
            Tickets.Add(ticket);
        }

        public static void UpdateTicket(Ticket updatedTicket)
        {
            var ticket = Tickets.FirstOrDefault(t => t.TicketID == updatedTicket.TicketID);
            if (ticket != null)
            {
                ticket.AssignedTo = updatedTicket.AssignedTo;
                ticket.Status = updatedTicket.Status;
                ticket.ResolvedAt = updatedTicket.ResolvedAt;
            }
        }
    }

}
