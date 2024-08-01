using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            return View();
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
    }
}
