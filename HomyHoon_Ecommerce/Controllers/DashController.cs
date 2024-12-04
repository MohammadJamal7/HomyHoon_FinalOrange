using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomyHoon_Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public DashController(ApplicationDbContext context, UserManager<User> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        public async Task<IActionResult> Apartment()
        {
            var apartments = await _context.Apartments.Include(ap=>ap.Category).ToListAsync();
            return View(apartments);
        }

        public async Task<IActionResult> Messages() {
            
            var messages = await _context.Messages.ToListAsync();

            return View(messages);
        }
        
        public async Task<IActionResult> Users() {

            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }


        public async Task<IActionResult> DeleteApart(int id)
        {
            try
            {
                var apartment = await _context.Apartments.FindAsync(id);

                if (apartment == null)
                {
                    TempData["ErrorMessage"] = "The apartment you are trying to delete does not exist.";
                    return RedirectToAction("Index", "Dash"); // Redirect to your desired action
                }

                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "The apartment was successfully deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the apartment: {ex.Message}";
            }

            return RedirectToAction("Apartment", "Dash"); // Redirect to your desired action
        }


        public async Task<IActionResult> Categories()
        {
            var caties = await _context.Categories.Include(cat=>cat.Apartments).ToListAsync();

            var viewModel = new CategoryVM
            {
                Categories = caties,
                Category = new Category()
            };
            return View(viewModel);
        }




        public IActionResult AddCategory()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCategory(IFormFile Image, string Name, string Description)
        {
            if (string.IsNullOrWhiteSpace(Name) || Image == null || string.IsNullOrWhiteSpace(Description))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return View();
            }

            try
            {
                // Save the image to wwwroot/images (or your preferred directory)
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                // Add the category to the database
                var category = new Category
                {
                    Name = Name,
                    Description = Description,
                    ImagePath = "/images/uploads/" + uniqueFileName
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Categories", "Dash");
        }





        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Apartments) // Assuming Apartments is a navigation property
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    TempData["ErrorMessage"] = "The category you are trying to delete does not exist.";
                    return RedirectToAction("Categories", "Dash");
                }

                if (category.Apartments.Any())
                {
                    TempData["ErrorMessage"] = "The category cannot be deleted because it is linked to apartments.";
                    return RedirectToAction("Categories", "Dash");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "The category was successfully deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the category: {ex.Message}";
            }

            return RedirectToAction("Categories", "Dash");
        }


    }
}
