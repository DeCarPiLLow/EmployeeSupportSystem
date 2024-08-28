using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;

using EmployeeSupportSystem.Models;

using EmployeeSupportSystem.Data;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace EmployeeSupportSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;

        public AccountController(ILogger<AccountController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]

        public IActionResult Login()

        {

            return View(); // Display the login page

        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel model)

        {

            if (ModelState.IsValid) // Check if the model is valid

            {

                // Validate the user using Employee ID and password
                UserData userdataobj = new UserData(_context);

                var user = userdataobj.ValidateUser(model.Id, model.Password);

                if (user != null)

                {

                    // Creating claims for the logged-in user

                    var claims = new List<Claim>

                    {

                        new Claim(ClaimTypes.Name, user.Id), // Using Employee ID instead of username for login

                        new Claim(ClaimTypes.Role, user.UserRole.ToString()) // Storing user role in claims

                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties { };

                    // Sign in the user

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect based on role

                    switch (user.UserRole)

                    {

                        case Role.Admin:

                            return RedirectToAction("AdminPage", "Home");

                        case Role.SupportAgent:

                            return RedirectToAction("SupportAgentPage", "Home");

                        case Role.Employee:

                            return RedirectToAction("EmployeePage", "Home");

                        default:

                            return RedirectToAction("Index", "Home");

                    }

                }

                else

                {

                    // Add an error message if credentials are invalid

                    ModelState.AddModelError(string.Empty, "Invalid employee ID or password.");

                }

            }

            return View(model); // Return the login page with validation errors, if any

        }

        [HttpGet]

        public IActionResult Signup()

        {

            return View(); // Display the signup page

        }

        [HttpPost]

        public IActionResult Signup(SignupViewModel model)

        {

            if (ModelState.IsValid) // Check if the model is valid

            {

                // Attempt to create a new user
                UserData userdataobj = new UserData(_context);
                var result = userdataobj.CreateUser(model.Id, model.Username, model.Password, out string errorMessage);

                if (result)

                {

                    // Redirect to the login page on successful signup

                    return RedirectToAction("Login");

                }

                // Add error message if user creation fails

                ModelState.AddModelError(string.Empty, errorMessage);

            }

            return View(model); // Return the signup page with validation errors, if any

        }

        [HttpPost]

        public async Task<IActionResult> Logout()

        {

            // Sign out the user

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to login page after logout

            return RedirectToAction("Login");

        }

    }

}