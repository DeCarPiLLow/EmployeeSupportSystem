namespace EmployeeSupportSystem.Data
{
    public class TicketData
    {
        private static List<Ticket> Tickets = new List<Ticket>(); // In-memory list to store tickets

        public static List<Ticket> GetAllTickets()
        {
            return Tickets; // Returns the list of all tickets
        }

        public static List<Ticket> GetTicketsByCreator(string creator)
        {
            // Returns tickets created by the specified user
            return Tickets.Where(ticket => ticket.CreatedBy == creator).ToList();
        }

        public static List<Ticket> GetTicketsByAssignee(string assignee)
        {
            // Returns tickets assigned to the specified user
            return Tickets.Where(ticket => ticket.AssignedTo == assignee).ToList();
        }

        public static void AddTicket(Ticket ticket)
        {
            Tickets.Add(ticket); // Adds a new ticket to the list
        }

        public static void UpdateTicket(Ticket updatedTicket)
        {
            var ticket = Tickets.FirstOrDefault(t => t.TicketID == updatedTicket.TicketID); // Find the ticket to update by ID
            if (ticket != null)
            {
                ticket.AssignedTo = updatedTicket.AssignedTo; // Update the assignee
                ticket.Status = updatedTicket.Status; // Update the status
                ticket.ResolvedAt = updatedTicket.ResolvedAt; // Update the resolved timestamp
            }
        }
    }
}
