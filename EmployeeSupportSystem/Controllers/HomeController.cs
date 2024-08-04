using EmployeeSupportSystem.Data; // Importing data-related functionality
using EmployeeSupportSystem.Models; // Importing models from the EmployeeSupportSystem namespace
using Microsoft.AspNetCore.Authorization; // Using ASP.NET Core authorization framework
using Microsoft.AspNetCore.Mvc; // Using ASP.NET Core MVC framework
using System.Diagnostics; // Using system diagnostics
using System.Net.Sockets; // Using network-related functionality
using System.Collections.Generic; // Using collections functionality

namespace EmployeeSupportSystem.Controllers
{
    [Authorize] // Ensures that all actions in this controller require authentication
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger for the HomeController

        // Constructor to initialize the logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(); // Returns the Index view
        }

        [Authorize(Roles = "Admin")] // Restricts access to users with the Admin role
        public IActionResult AdminPage()
        {
            var ticket = TicketData.GetAllTickets(); // Get all tickets
            var user = UserData.GetAllUsers(); // Get all users
            var viewModel = new AdminViewModel
            {
                Tickets = ticket,
                Users = user,
            };
            return View(viewModel); // Returns the AdminPage view with the view model
        }

        [Authorize(Roles = "SupportAgent")] // Restricts access to users with the SupportAgent role
        public IActionResult SupportAgentPage()
        {
            var tickets = TicketData.GetTicketsByAssignee(User.Identity.Name); // Get tickets assigned to the current user
            var resolvedTickets = tickets.Where(t => t.Status == TicketStatus.Resolved).ToList(); // Filter resolved tickets
            var pendingTickets = tickets.Where(t => t.Status != TicketStatus.Resolved).ToList(); // Filter pending tickets
            var viewModel = new SupportAgentViewModel
            {
                ResolvedTickets = resolvedTickets,
                PendingTickets = pendingTickets
            };
            return View(viewModel); // Returns the SupportAgentPage view with the view model
        }

        [Authorize(Roles = "Employee")] // Restricts access to users with the Employee role
        public IActionResult EmployeePage()
        {
            var tickets = TicketData.GetTicketsByCreator(User.Identity.Name); // Get tickets created by the current user
            return View(tickets); // Returns the EmployeePage view with the tickets
        }

        [Authorize(Roles = "Employee")] // Restricts access to users with the Employee role
        public IActionResult CreateNewTicket()
        {
            return View(new CreateNewTicketViewModel()); // Returns the CreateNewTicket view with an empty view model
        }

        [HttpPost] // Handles POST requests
        [Authorize(Roles = "Employee")] // Restricts access to users with the Employee role
        public IActionResult CreateNewTicket(CreateNewTicketViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                // Create a new ticket with the provided details
                var ticket = new Ticket
                {
                    TicketID = Guid.NewGuid().ToString(),
                    CreatedBy = User.Identity.Name,
                    Subject = model.Subject,
                    Description = model.Description,
                    Status = TicketStatus.Pending,
                    CreatedAt = DateTime.Now,
                };
                TicketData.AddTicket(ticket); // Add the ticket to the data store
                return RedirectToAction("EmployeePage"); // Redirect to EmployeePage after creating the ticket
            }
            return View(model); // Return view with model if creation fails
        }

        [HttpPost] // Handles POST requests
        [Authorize(Roles = "Admin")] // Restricts access to users with the Admin role
        public IActionResult AssignTicket(string ticketId, string assignee)
        {
            var ticket = TicketData.GetAllTickets().FirstOrDefault(t => t.TicketID == ticketId); // Find the ticket by ID
            if (ticket != null)
            {
                ticket.AssignedTo = assignee; // Assign the ticket to a user
                ticket.Status = TicketStatus.Assigned; // Update the ticket status
                TicketData.UpdateTicket(ticket); // Update the ticket in the data store
            }
            return RedirectToAction("AdminPage"); // Redirect to AdminPage after assigning the ticket
        }

        [HttpPost] // Handles POST requests
        [Authorize(Roles = "SupportAgent")] // Restricts access to users with the SupportAgent role
        public IActionResult UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = TicketData.GetAllTickets().FirstOrDefault(t => t.TicketID == ticketId); // Find the ticket by ID
            if (ticket != null)
            {
                ticket.Status = status; // Update the ticket status
                if (status == TicketStatus.InProgress)
                {
                    ticket.InProgressAt = DateTime.Now; // Set the when the support start working on it
                }
                if (status == TicketStatus.Resolved)
                {
                    ticket.ResolvedAt = DateTime.Now; // Set the resolved timestamp if the status is resolved
                }
                TicketData.UpdateTicket(ticket); // Update the ticket in the data store
            }
            return RedirectToAction("SupportAgentPage"); // Redirect to SupportAgentPage after updating the ticket status
        }

        [Authorize(Roles = "Admin")] // Restricts access to users with the Admin role
        public IActionResult ListUsers()
        {
            var users = UserData.GetAllUsers(); // Get all users
            return View(users); // Returns the ListUsers view with the users
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching for the Error view
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Returns the Error view with the request ID
        }
    }
}
