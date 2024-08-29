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
        // Logger for logging messages and errors
        private readonly ILogger<AccountController> _logger;
        // Database context for interacting with the data source
        private readonly DataContext _context;

        // Constructor to initialize the logger and context dependencies
        public AccountController(ILogger<AccountController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET method to display the login page
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Display the login page
        }

        // POST method to handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // Check if the form model is valid
            {
                // Validate the user using Employee ID and password
                UserData userdataobj = new UserData(_context); // Use UserData for validation
                var user = userdataobj.ValidateUser(model.Id, model.Password);

                if (user != null) // If user exists
                {
                    // Creating claims for the logged-in user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Id), // Employee ID claim for identification
                        new Claim(ClaimTypes.Role, user.UserRole.ToString()) // User role claim for role-based authorization
                    };

                    // Create identity based on the claims
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Authentication properties for cookie-based authentication
                    var authProperties = new AuthenticationProperties { };

                    // Sign in the user using the created claims and authentication properties
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect to different pages based on user role
                    switch (user.UserRole)
                    {
                        case Role.Admin:
                            return RedirectToAction("AdminPage", "Home"); // Redirect Admin users to AdminPage

                        case Role.SupportAgent:
                            return RedirectToAction("SupportAgentPage", "Home"); // Redirect Support Agents to their page

                        case Role.Employee:
                            return RedirectToAction("EmployeePage", "Home"); // Redirect Employees to their page

                        default:
                            return RedirectToAction("Index", "Home"); // Default redirect if role is unrecognized
                    }
                }
                else
                {
                    // If the user validation fails, display an error message
                    ModelState.AddModelError(string.Empty, "Invalid employee ID or password.");
                }
            }

            return View(model); // Return the login page with any validation errors
        }

        // GET method to display the signup page
        [HttpGet]
        public IActionResult Signup()
        {
            return View(); // Display the signup page
        }

        // POST method to handle signup form submission
        [HttpPost]
        public IActionResult Signup(SignupViewModel model)
        {
            if (ModelState.IsValid) // Check if the model is valid
            {
                // Create a new user using UserData helper class
                UserData userdataobj = new UserData(_context);
                var result = userdataobj.CreateUser(model.Id, model.Username, model.Password, out string errorMessage);

                if (result) // If user creation succeeds
                {
                    return RedirectToAction("Login"); // Redirect to the login page
                }

                // If user creation fails, display an error message
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return View(model); // Return the signup page with any validation errors
        }

        // POST method to handle user logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user from the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the login page after logout
            return RedirectToAction("Login");
        }
    }
}
