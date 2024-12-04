using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace HomyHoon_Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User>signinmanager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signinmanager;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationViewModel model, IFormFile ProfilePic)
        {

            if (true)
            {


                // Handle file upload if a file is provided
                
                
                    // Save the image to wwwroot/images (or your preferred directory)
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilePic.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePic.CopyToAsync(stream);
                    }
                

                // Create the user
                User usr = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    City = model.City,
                    Adress = model.Adress,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ProfilePicture = "/images/uploads/" + uniqueFileName
                };
            
                // Create the user with the provided password
                var result = await _userManager.CreateAsync(usr, model.Password);
                if (result.Succeeded)
                {
                    // Assign a default role (e.g., User)
                    var roleResult = await _userManager.AddToRoleAsync(usr, "User");

                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }


            // Return the model if validation fails
            return View(model);
        }




        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool rememberme= false)
        {
           
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("email", "Email is required.");
            }
            else if (!IsValidEmail(Email))
            {
                ModelState.AddModelError("email", "Please enter a valid email address.");
            }

           
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }

            
            if (!ModelState.IsValid)
            {
                return View();
            }

           
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Password, rememberme, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    TempData["SuccessMessage"] = "You have logged in successfully!";
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Users", "Dash");
                    }

                    return RedirectToAction("Index", "Pages");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "This account is locked out due to multiple failed login attempts.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No account found with the provided email.");
            }

            return View();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                return RedirectToAction("Users", "Dash");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users","Dash");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToAction("Users", "Dash");
        }

        public async Task<IActionResult> SignOut()
        {
            
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
             
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Users", "Dash");
                }
                else if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Pages");
                }
            }

            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Pages");
        }


        public async Task<IActionResult> AccessDenied()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            ViewData["Role"] = userRoles.FirstOrDefault();
            return View();
        }

    }
}
