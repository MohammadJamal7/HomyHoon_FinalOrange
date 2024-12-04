using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomyHoon_Ecommerce.Controllers
{
    public class ApartmentsController : Controller


    {
        private readonly ApplicationDbContext _context;
        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
      string categoryName = null,
      int bedrooms = 0,
      int bathrooms = 0,
      int page = 1,
      int pageSize = 9,
      string city = null,
      string furnished = null,
      bool showOnlyReserved = false)
        {
            var today = DateTime.Today;

           
            var apartments = _context.Apartments.AsQueryable();

            
            if (showOnlyReserved)
            {
                apartments = apartments.Where(ap => _context.Bookings
                    .Any(booking => booking.ApartmentId == ap.Id &&
                                    booking.Apartment.IsAvailable ==false &&
                                    booking.CheckOutDate >= today));
            }
            else
            {
                apartments = apartments.Where(ap => (ap.AvailableFrom == null || ap.AvailableFrom <= today) &&
                                                     (ap.AvailableUntil == null || ap.AvailableUntil >= today) &&
                                                     ap.IsAvailable);
            }

            // Other filters
            if (!string.IsNullOrEmpty(categoryName))
            {
                apartments = apartments.Where(a => a.Category.Name == categoryName);
            }

            if (bedrooms > 0)
            {
                apartments = apartments.Where(a => a.Beds == bedrooms);
            }

            if (bathrooms > 0)
            {
                apartments = apartments.Where(a => a.Baths == bathrooms);
            }

            if (!string.IsNullOrEmpty(city))
            {
                apartments = apartments.Where(a => a.City == city);
            }

            if (!string.IsNullOrEmpty(furnished))
            {
                apartments = apartments.Where(a => a.IsFrunshised == (furnished == "Yes"));
            }

            apartments = apartments.Include(ap => ap.Images).Include(ap => ap.Category);

            // Pagination
            var totalItems = await apartments.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var apartmentsOnPage = await apartments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Prepare view model
            var viewModel = new ApartmentListViewModel
            {
                Apartments = apartmentsOnPage,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                CategoryFilter = categoryName,
                BedroomsFilter = bedrooms,
                BathroomsFilter = bathrooms,
                CityFilter = city,
                FurnishedFilter = furnished,
                IncludeReserved = showOnlyReserved,
                categories = await _context.Categories.ToListAsync(),
            };

            ViewBag.ResetUrl = Url.Action("Index", "Apartments");

            return View(viewModel);
        }





    }
}
