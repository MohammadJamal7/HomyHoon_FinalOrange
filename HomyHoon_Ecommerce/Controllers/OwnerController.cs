using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;

namespace HomyHoon_Ecommerce.Controllers
{
    [Authorize(Roles ="User")]
    public class OwnerController : Controller
    {
        private readonly  UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public OwnerController(ApplicationDbContext context,UserManager<User> manager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = manager;
            _hostEnvironment = hostEnvironment;
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var OwnerApartments = await _context.Apartments.Where(ap=>ap.UserId == user!.Id).ToListAsync();

            var TotalAparts = OwnerApartments.Count();
            var TotalBookings = _context.Apartments
           .Where(ap => ap.UserId == user.Id)
           .SelectMany(ap => ap.Bookings)
           .Count();

            var TotalRevenue = await _context.Bookings
    .Where(b => b.Apartment.UserId == user.Id) // Filter by apartments owned by the user
    .SumAsync(b => b.TotalPrice);

            var viewmodel = new OwnerVM
            {
                OwnerAparts = OwnerApartments,
                TotalApart = TotalAparts,
                TotalBookings = TotalBookings,
                TotalReveniew = TotalRevenue,
                User = user,
            };

            return View(viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> AddApartment()
        {
            var categories = await _context.Categories.ToListAsync();
            var viwModel = new AddApartmentVM
            {
                Categories = categories,
                Apartment = new Apartment(),
            };

            if (!categories.Any())
            {
                ViewData["ErrorMessage"] = "No categories available. Please add categories first.";
            }
            return View(viwModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddApartment( AddApartmentVM model, List<IFormFile> images)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.Apartment.CategoryId);

                

                model.Apartment.User = user;
                model.Apartment.Category = category;

                
                var apartmentImages = new List<ApartmentImage>();
                if (images != null && images.Any())
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "apartments");
                    Directory.CreateDirectory(uploadPath); 

                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                           
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                            var filePath = Path.Combine(uploadPath, fileName);

                          
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                           
                            var apartmentImage = new ApartmentImage
                            {
                                ImageUrl = $"/uploads/apartments/{fileName}"
                            };
                            apartmentImages.Add(apartmentImage);
                        }
                    }

                  
                    if (apartmentImages.Any())
                    {
                        model.Apartment.MainImage = apartmentImages.First().ImageUrl;
                    }
                }

                model.Apartment.Images = apartmentImages;
                _context.Apartments.Add(model.Apartment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Owner");
            }
            catch (Exception ex)
            {
             
                ModelState.AddModelError("", "An error occurred while saving the apartment.");
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }
        }

       
        public async Task<IActionResult> Delete(int? id=1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var apartment = await _context.Apartments
                .Include(a => a.Bookings) 
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

           
            _context.Bookings.RemoveRange(apartment.Bookings);

           
            _context.Apartments.Remove(apartment);

            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Apartment and related bookings deleted successfully!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateApartment(int id)
        {
            
            var apartment = await _context.Apartments
                .Include(a => a.Category).Include(apartment => apartment.Images) 
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                TempData["ErrorMessage"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

         
            var categories = await _context.Categories.ToListAsync();

           
            var viewModel = new UpdateApartmentVM
            {
                Id = apartment.Id,
                Name = apartment.Name,
                CategoryId = apartment.CategoryId,
                PricePerNight = apartment.PricePerNight,
                Rooms = apartment.Rooms,
                Beds = apartment.Beds,
                Baths = apartment.Baths,
                MaxGuests = apartment.MaxGuests,
                Description = apartment.Description,
                IsFrunshised = apartment.IsFrunshised,
                Kitchen = apartment.Kitchen,
                FirstAid = apartment.FirstAid,
                TV = apartment.TV,
                Wifi = apartment.Wifi,
                Parking = apartment.Parking,
                AirConditioning = apartment.AirConditioning,
                Heating = apartment.Heating,
                MainImage = apartment.MainImage,
                Address = apartment.Address,
                City = apartment.City,
                Categories = categories,
                Images = apartment.Images,
            };
            Console.Write("teting");
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateApartment(UpdateApartmentVM model, List<IFormFile> Images, string DeletedImages)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.Categories = await _context.Categories.ToListAsync();
            //    return View(model);
            //}

            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == model.Id);

            if (apartment == null)
            {
                TempData["ErrorMessage"] = "Apartment not found.";
                return RedirectToAction("Index");
            }

            // Update apartment details
            apartment.Name = model.Name;
            apartment.CategoryId = (int)model.CategoryId;
            apartment.PricePerNight = model.PricePerNight;
            apartment.Rooms = model.Rooms;
            apartment.Beds = model.Beds;
            apartment.Baths = model.Baths;
            apartment.MaxGuests = model.MaxGuests;
            apartment.Description = model.Description;

            // Delete removed images
            if (!string.IsNullOrEmpty(DeletedImages))
            {
                var deletedImageIds = DeletedImages.Split(',').Select(int.Parse).ToList();
                var imagesToDelete = apartment.Images.Where(i => deletedImageIds.Contains(i.Id)).ToList();
                foreach (var image in imagesToDelete)
                {
                    // Remove from database
                    _context.ApartmentImages.Remove(image);

                    // Delete from filesystem
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            // Add new images
            if (Images != null && Images.Any())
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var img in Images)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + img.FileName;
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }

                    var apartmentImage = new ApartmentImage
                    {
                        ImageUrl = "/images/uploads/" + fileName,
                        ApartmentId = apartment.Id
                    };

                    apartment.Images.Add(apartmentImage);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Apartment updated successfully!";
            return RedirectToAction("Index");
        }




    }
}