using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HomyHoon_Ecommerce.Controllers
{
    public class Reservation : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public Reservation(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int ApartmentId, DateTime CheckIn, DateTime CheckOut, int Guests , string ApartmentName)
        {
            var apartment = await _context.Apartments.FirstOrDefaultAsync(ap => ap.Id == ApartmentId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Ensure availability dates are set
            if (apartment.AvailableFrom == null || apartment.AvailableUntil == null)
            {
                apartment.AvailableFrom = DateTime.Today;
                apartment.AvailableUntil = DateTime.MaxValue;
            }

            // Validate reservation dates
            if (CheckIn < DateTime.Today || CheckOut <= CheckIn || CheckIn < apartment.AvailableFrom || CheckOut > apartment.AvailableUntil)
            {
                TempData["Error"] = "Invalid reservation dates.";
                return RedirectToAction("ApartmentDetails", "Pages", new { id = ApartmentId });
            }

            // Create the booking
            var booking = new Booking
            {
                CheckInDate = CheckIn,
                CheckOutDate = CheckOut,
                TotalPrice = (double)((CheckOut - CheckIn).Days * apartment.PricePerNight),
                Apartment = apartment,
                User = currentUser,
                NumberOfGuests = Guests,
                BookingStatus = "Pending",
            };

            apartment.AvailableFrom = CheckOut; 
            

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var bookingJson = JsonConvert.SerializeObject(booking, settings);
            HttpContext.Session.SetString("Reservation", bookingJson);
            Console.WriteLine($"Booking stored in session: {bookingJson}");
            return RedirectToAction("Payment", "Reservation" , new { booking.TotalPrice});
        }


        public IActionResult Payment(double TotalPrice)
        {
            ViewData["TotalPrice"] = TotalPrice;
            return View(TotalPrice);
        }



        public async Task<IActionResult> FinishPayment()
        {
            // Retrieve the reservation from the session
            var reservationJson = HttpContext.Session.GetString("Reservation");

            if (string.IsNullOrEmpty(reservationJson))
            {
                Console.WriteLine("No reservation found in session.");
                return RedirectToAction("Error", "Page");
            }

            Console.WriteLine($"Retrieved reservation from session: {reservationJson}");

            // Deserialize the booking object
            var booking = JsonConvert.DeserializeObject<Booking>(reservationJson);

            // Fetch existing related entities from the database to avoid tracking conflicts
            var existingApartment = await _context.Apartments.FindAsync(booking.Apartment.Id);
            var existingUser = await _userManager.FindByIdAsync(booking.User.Id);

            if (existingApartment == null || existingUser == null)
            {
                Console.WriteLine("Related entities not found in the database.");
                return RedirectToAction("Error", "Page");
            }

            // Assign the existing entities to the booking to avoid tracking new instances
            booking.Apartment = existingApartment;
            booking.User = existingUser;

            // Update the booking status and apartment availability
            booking.BookingStatus = "Completed";
            existingApartment.IsAvailable = false;
            existingApartment.AvailableFrom = booking.CheckOutDate; // Set the next available date

            // Remove the reservation from session
            HttpContext.Session.Remove("Reservation");

            // Add the booking to the database
            await _context.Bookings.AddAsync(booking);

            // Save all changes, including updates to the apartment
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Pages");
        }




    }
}
