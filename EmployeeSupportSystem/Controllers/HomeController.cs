using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Sockets;
using System.Collections.Generic;

namespace EmployeeSupportSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            var ticket = TicketData.GetAllTickets();
            var user = UserData.GetAllUsers();
            var viewModel = new AdminViewModel
            {
                Tickets = ticket,
                Users = user,
            };
            return View(viewModel);
        }

        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAgentPage()
        {
            var tickets = TicketData.GetTicketsByAssignee(User.Identity.Name);
            var resolvedTickets = tickets.Where(t => t.Status == TicketStatus.Resolved).ToList();
            var pendingTickets = tickets.Where(t => t.Status != TicketStatus.Resolved).ToList();
            var viewModel = new SupportAgentViewModel
            {
                ResolvedTickets = resolvedTickets,
                PendingTickets = pendingTickets
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeePage()
        {
            var tickets = TicketData.GetTicketsByCreator(User.Identity.Name);
            return View(tickets);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult CreateNewTicket()
        {
            return View(new CreateNewTicketViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult CreateNewTicket(CreateNewTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = new Ticket
                {
                    TicketID = Guid.NewGuid().ToString(),
                    CreatedBy = User.Identity.Name,
                    Subject = model.Subject,
                    Description = model.Description,
                    Status = TicketStatus.Pending,
                    CreatedAt = DateTime.Now,
                };
                TicketData.AddTicket(ticket);
                return RedirectToAction("EmployeePage");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignTicket(string ticketId, string assignee)
        {
            var ticket = TicketData.GetAllTickets().FirstOrDefault(t => t.TicketID == ticketId);
            if (ticket != null)
            { 
                ticket.AssignedTo = assignee;
                ticket.Status = TicketStatus.Assigned; 
                TicketData.UpdateTicket(ticket); 
            }
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        [Authorize(Roles = "SupportAgent")]
        public IActionResult UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = TicketData.GetAllTickets().FirstOrDefault(t => t.TicketID == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;
                if (status == TicketStatus.Resolved)
                {
                    ticket.ResolvedAt = DateTime.Now;
                }
                TicketData.UpdateTicket(ticket);
            }
            return RedirectToAction("SupportAgentPage");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = UserData.GetAllUsers();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
