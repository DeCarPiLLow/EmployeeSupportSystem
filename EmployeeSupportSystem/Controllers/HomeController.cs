using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace EmployeeSupportSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            var tickets = _context.Tickets.ToList();
            var users = _context.Users.ToList();
            var viewModel = new AdminViewModel
            {
                Tickets = tickets,
                Users = users
            };
            return View(viewModel);
        }

        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAgentPage()
        {
            var tickets = _context.Tickets
                .Where(t => t.AssignedTo == User.Identity.Name)
                .ToList();

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
            var tickets = _context.Tickets
                .Where(t => t.CreatedBy == User.Identity.Name)
                .ToList();

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
                    CreatedAt = DateTime.Now
                };
                _context.Tickets.Add(ticket);
                _context.SaveChanges();

                return RedirectToAction("EmployeePage");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignTicket(string ticketId, string assignee)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == ticketId);
            if (ticket != null)
            {
                ticket.AssignedTo = assignee;
                ticket.Status = TicketStatus.Assigned;
                ticket.AssignedAt = DateTime.Now;
                _context.Tickets.Update(ticket);
                _context.SaveChanges();
            }

            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        [Authorize(Roles = "SupportAgent")]
        public IActionResult UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;

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

                _context.Tickets.Update(ticket);
                _context.SaveChanges();
            }

            return RedirectToAction("SupportAgentPage");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Analytics()
        {
            var userId = User.Identity.Name;
            var tickets = _context.Tickets
                .Where(t => t.CreatedBy == userId)
                .ToList();

            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminAnalytics()
        {
            var tickets = _context.Tickets.ToList();

            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAnalytics()
        {
            var userId = User.Identity.Name;
            var tickets = _context.Tickets
                .Where(t => t.AssignedTo == userId)
                .ToList();

            var viewModel = tickets.Select(t => new TicketAnalyticsViewModel
            {
                TicketID = t.Subject,
                TimePending = t.AssignedAt.HasValue ? (t.AssignedAt.Value - t.CreatedAt).TotalHours : 0,
                TimeAllocated = t.ActiveAt.HasValue && t.AssignedAt.HasValue ? (t.ActiveAt.Value - t.AssignedAt.Value).TotalHours : 0,
                TimeActive = t.ResolvedAt.HasValue && t.ActiveAt.HasValue ? (t.ResolvedAt.Value - t.ActiveAt.Value).TotalHours : 0,
                TimeResolved = t.ResolvedAt.HasValue ? (t.ResolvedAt.Value - t.CreatedAt).TotalHours : 0
            }).ToList();

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}