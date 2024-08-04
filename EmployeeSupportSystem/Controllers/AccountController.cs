using Microsoft.AspNetCore.Mvc; // Using ASP.NET Core MVC framework
using Microsoft.AspNetCore.Authentication; // Using ASP.NET Core authentication framework
using Microsoft.AspNetCore.Authentication.Cookies; // Using cookie-based authentication
using System.Security.Claims; // Using claims-based identity
using EmployeeSupportSystem.Models; // Importing models from the EmployeeSupportSystem namespace
using EmployeeSupportSystem.Data; // Importing data-related functionality

namespace EmployeeSupportSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger; // Logger for the AccountController

        // Constructor to initialize the logger
        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Returns the Login view
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                var user = UserData.ValidateUser(model.Id, model.Password); // Validate user credentials
                if (user != null)
                {
                    // Create claims for the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.UserRole.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // Create claims identity
                    var authProperties = new AuthenticationProperties { }; // Authentication properties

                    // Sign in the user with the claims identity
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect based on the user role
                    switch (user.UserRole)
                    {
                        case Role.Admin:
                            return RedirectToAction("AdminPage", "Home"); // Redirect to Admin page
                        case Role.SupportAgent:
                            return RedirectToAction("SupportAgentPage", "Home"); // Redirect to Support Agent page
                        case Role.Employee:
                            return RedirectToAction("EmployeePage", "Home"); // Redirect to Employee page
                        default:
                            return RedirectToAction("Index", "Home"); // Redirect to default page
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt!!"); // Add error message if login fails
            }
            return View(model); // Return view with model if login fails
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View(); // Returns the Signup view
        }

        [HttpPost]
        public IActionResult Signup(SignupViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                var result = UserData.CreateUser(model.Id, model.Username, model.Password, out string errorMessage); // Create a new user
                if (result)
                {
                    return RedirectToAction("Login"); // Redirect to Login page on success
                }
                ModelState.AddModelError(string.Empty, errorMessage); // Add error message if user creation fails
            }
            return View(model); // Return view with model if signup fails
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Sign out the user
            return RedirectToAction("Login"); // Redirect to Login page after logout
        }
    }
}
