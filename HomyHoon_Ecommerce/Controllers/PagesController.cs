using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomyHoon_Ecommerce.Controllers
{
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext _context;


        public PagesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            IndexPageViewModel models = new IndexPageViewModel
            {
                Categories = await _context.Categories.ToListAsync(),

            };

            await UpdateAvailability();
            return View(models);
        }


        public async Task<IActionResult> ApartmentsListing()
        {
            var today = DateTime.Today;

            // Fetch apartments that are available and not reserved
            var apartments = await _context.Apartments
                .Where(ap => ap.AvailableFrom <= today
                             && ap.AvailableUntil >= today
                             && ap.IsAvailable
                             && !_context.Bookings
                                 .Any(booking => booking.ApartmentId == ap.Id
                                                 && booking.CheckInDate <= today
                                                 && booking.CheckOutDate >= today))
                .Include(ap => ap.Images) // Include related images if necessary
                .ToListAsync();

            return View(apartments);
        }

        public IActionResult AboutUs()
        {
            return View();
        }


        public async Task<IActionResult> ApartmentDetails(int id)
        {
            // Fetch the apartment details with related images
            var apartment = await _context.Apartments
                .Include(ap => ap.Images)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            bool isReserved = apartment.AvailableFrom.HasValue && apartment.AvailableFrom.Value > today;

            string availableDateMessage = isReserved
                ? $"This apartment will be available from {apartment.AvailableFrom.Value.ToString("MMMM dd, yyyy")}."
                : "This apartment is currently available.";

           
            var viewModel = new DetailsViewModel
            {
                Apartment = apartment,
                IsReserved = isReserved,
                AvailableFrom = apartment.AvailableFrom,
                AvailableDateMessage = availableDateMessage
            };

            return View(viewModel);
        }




        private async Task UpdateAvailability()
        {
            var expiredReservations = await _context.Bookings
                .Where(b => b.CheckOutDate <= DateTime.UtcNow && !b.Apartment.IsAvailable)
                .Include(b => b.Apartment)
                .ToListAsync();

            foreach (var booking in expiredReservations)
            {
                var apartment = booking.Apartment;
                apartment.IsAvailable = true;

                // Reset availability fields
                apartment.AvailableFrom = null;
                apartment.AvailableUntil = null;
            }

            await _context.SaveChangesAsync();
        }

    }
}
