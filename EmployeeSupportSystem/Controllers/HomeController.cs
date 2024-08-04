using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            return View();
        }

        [Authorize(Roles = "SupportAgent")]
        public IActionResult SupportAgentPage()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeePage()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            var ticket = UserData.GetTicketsByUser(userId);

            var viewModel = new EmployeeDashboardViewModel
            {
                Tickets = ticket,
                NewTicket = new NewTicketViewModel()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateTicket(NewTicketViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            var ticket = new Ticket
            {
                Id = System.Guid.NewGuid().ToString(),
                UserId = userId,
                Subject = model.Subject,
                Description = model.Description,
                Status = TicketStatus.Pending,
            };

            UserData.AddTicket(ticket);
            return RedirectToAction("EmployeePage");
        }

        public IActionResult TicketDetails(string id)
        {
            var ticket = UserData.GetTicketsByUser(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value).FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

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
        public IActionResult Analytics() { 
            return View();
        }
    }
}
