namespace EmployeeSupportSystem.Data
{
    public class TicketData
    {
        private readonly DataContext _context;

        public TicketData(DataContext context)
        {
            _context = context;
        }

        public List<Ticket> GetAllTickets()
        {
            return _context.Tickets.ToList(); // Returns the list of all tickets
        }

        public List<Ticket> GetTicketsByCreator(string creator)
        {
            // Returns tickets created by the specified user
            return _context.Tickets.Where(ticket => ticket.CreatedBy == creator).ToList();
        }

        public List<Ticket> GetTicketsByAssignee(string assignee)
        {
            // Returns tickets assigned to the specified user
            return _context.Tickets.Where(ticket => ticket.AssignedTo == assignee).ToList();
        }

        public void AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket); // Adds a new ticket to the list
            _context.SaveChanges();
        }

        public void UpdateTicket(Ticket updatedTicket)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == updatedTicket.TicketID);// Find the ticket to update by ID
            if (ticket != null)
            {
                ticket.AssignedTo = updatedTicket.AssignedTo; // Update the assignee
                ticket.Status = updatedTicket.Status; // Update the status
                ticket.ResolvedAt = updatedTicket.ResolvedAt; // Update the resolved timestamp
                if (updatedTicket.Status == TicketStatus.Assigned)
                {
                    ticket.AssignedAt = DateTime.Now;
                }
                if (updatedTicket.Status == TicketStatus.Active)
                {
                    ticket.ActiveAt = DateTime.Now;
                }
                _context.SaveChanges();
            }
        }
    }
}
