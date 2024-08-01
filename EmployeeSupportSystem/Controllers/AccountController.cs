using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using EmployeeSupportSystem.Models;
using EmployeeSupportSystem.Data;

namespace EmployeeSupportSystem.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserData.ValidateUser(model.Username, model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.UserRole.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    switch (user.UserRole)
                    {
                        case Role.Admin:
                            return RedirectToAction("AdminPage", "Home");
                        case Role.SupportAgent:
                            return RedirectToAction("SupportAgentPage", "Home");
                        case Role.Empolyee:
                            return RedirectToAction("EmployeePage", "Home");
                        default:
                            return RedirectToAction("Index", "Home");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attemp.");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
