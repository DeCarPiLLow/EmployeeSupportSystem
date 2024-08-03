using EmployeeSupportSystem.Data;
using EmployeeSupportSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeSupportSystem.Controllers
{
    public class TicketController : Controller
    {

        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTicket(TicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var result = TicketData.CreateNewicket(model.UserId, model.Subject, model.Description);
                if (result)
                {
                    return RedirectToAction("EmployeePage", "Home");
                }
            }
            return View(model);
        }

        public IActionResult ListTickets()
        {
            var tickets = TicketData.GetAllTickets();
            return View(tickets);
        }
    }
}
