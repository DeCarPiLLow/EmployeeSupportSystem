using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeSupportSystem.Controllers
{
    // Enforces authorization for all actions in the controller
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        // Constructor to inject logger and data context dependencies
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Displays the default home page
        public IActionResult Index()
        {
            return View();
        }

        // Displays the admin dashboard; only accessible to users with the Admin role
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            var tickets = _context.Tickets.ToList(); // Get all tickets
            var users = _context.Users.ToList(); // Get all users
            var viewModel = new AdminViewModel
            {
                Tickets = tickets,
                Users = users
            };
            return View(viewModel); // Passes data to the view
        }

        // Displays the support agent dashboard; only accessible to users with the SupportAgent role
        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAgentPage()
        {
            var tickets = _context.Tickets
                .Where(t => t.AssignedTo == User.Identity.Name) // Get tickets assigned to the logged-in user
                .ToList();

            // Split tickets into resolved and pending based on their status
            var resolvedTickets = tickets.Where(t => t.Status == TicketStatus.Resolved).ToList();
            var pendingTickets = tickets.Where(t => t.Status != TicketStatus.Resolved).ToList();

            var viewModel = new SupportAgentViewModel
            {
                ResolvedTickets = resolvedTickets,
                PendingTickets = pendingTickets
            };

            return View(viewModel); // Passes data to the view
        }

        // Displays the employee dashboard; only accessible to users with the Employee role
        [Authorize(Roles = "Employee")]
        public IActionResult EmployeePage()
        {
            var tickets = _context.Tickets
                .Where(t => t.CreatedBy == User.Identity.Name) // Get tickets created by the logged-in user
                .ToList();

            return View(tickets); // Passes data to the view
        }

        // Renders a form for creating a new ticket; only accessible to users with the Employee role
        [Authorize(Roles = "Employee")]
        public IActionResult CreateNewTicket()
        {
            return View(new CreateNewTicketViewModel());
        }

        // Handles form submission for creating a new ticket; only accessible to users with the Employee role
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult CreateNewTicket(CreateNewTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new ticket and populate fields
                var ticket = new Ticket
                {
                    TicketID = "TICKET-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedBy = User.Identity.Name,
                    Subject = model.Subject,
                    Description = model.Description,
                    Status = TicketStatus.Pending,
                    CreatedAt = DateTime.Now
                };
                _context.Tickets.Add(ticket); // Add the new ticket to the database
                _context.SaveChanges(); // Save changes

                return RedirectToAction("EmployeePage"); // Redirect to the employee dashboard
            }

            return View(model); // If model validation fails, return the form with errors
        }

        //[HttpPost]
        //[Authorize(Roles = "Employee")]
        //public IActionResult ApplyForAgent()
        //{

        //}

        // Assigns a ticket to a user (assignee); only accessible to users with the Admin role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignTicket(string ticketId, string assignee)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == ticketId); // Find the ticket by ID
            if (ticket != null)
            {
                ticket.AssignedTo = assignee; // Assign the ticket
                ticket.Status = TicketStatus.Assigned; // Update the status
                ticket.AssignedAt = DateTime.Now; // Set the assignment time
                _context.Tickets.Update(ticket); // Update the ticket in the database
                _context.SaveChanges(); // Save changes
            }

            return RedirectToAction("AdminPage"); // Redirect to the admin dashboard
        }

        // Changes role of an employee to support agent; only accessible to users with the Admin role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUserRole(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null && user.UserRole == Role.Employee)
            {
                user.UserRole = Role.SupportAgent;
                _context.Users.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"{user.Username} has been successfully changed to a Support Agent.";
            }

            return RedirectToAction("ListUsers");
        }

        // Updates the status of a ticket; only accessible to users with the SupportAgent role
        [HttpPost]
        [Authorize(Roles = "SupportAgent")]
        public IActionResult UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == ticketId); // Find the ticket by ID
            if (ticket != null)
            {
                ticket.Status = status; // Update the ticket status

                // Set timestamps based on the status
                if (status == TicketStatus.Assigned)
                {
                    ticket.AssignedAt = DateTime.Now;
                }
                else if (status == TicketStatus.InProgress)
                {
                    ticket.ActiveAt = DateTime.Now;
                }
                else if (status == TicketStatus.Resolved)
                {
                    ticket.ResolvedAt = DateTime.Now;
                }

                _context.Tickets.Update(ticket); // Update the ticket in the database
                _context.SaveChanges(); // Save changes
            }

            return RedirectToAction("SupportAgentPage"); // Redirect to the support agent dashboard
        }

        // Displays a list of all users; only accessible to users with the Admin role
        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = _context.Users.ToList(); // Get all users
            return View(users); // Passes data to the view
        }

        // Displays analytics for the logged-in employee; only accessible to users with the Employee role
        [Authorize(Roles = "Employee")]
        public IActionResult Analytics()
        {
            var userId = User.Identity.Name; // Get the logged-in user's ID
            var tickets = _context.Tickets
                .Where(t => t.CreatedBy == userId) // Get tickets created by the user
                .ToList();

            // Calculate ticket times and create a view model for each ticket
            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel); // Passes data to the view
        }

        // Displays admin analytics; only accessible to users with the Admin role
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAnalytics()
        {
            var tickets = _context.Tickets.ToList(); // Get all tickets

            // Calculate ticket times and create a view model for each ticket
            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel); // Passes data to the view
        }

        // Displays support agent analytics; only accessible to users with the SupportAgent role
        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAnalytics()
        {
            var userId = User.Identity.Name; // Get the logged-in user's ID
            var tickets = _context.Tickets
                .Where(t => t.AssignedTo == userId) // Get tickets assigned to the user
                .ToList();

            // Calculate ticket times and create a view model for each ticket
            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel); // Passes data to the view
        }

        // Handles application errors and returns the error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
