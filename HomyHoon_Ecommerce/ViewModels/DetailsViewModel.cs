using HomyHoon.Models;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class DetailsViewModel
    {

        public Apartment Apartment { get; set; }
        public Booking Booking { get; set; }
        public bool IsReserved { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public string AvailableDateMessage { get; set; }
    }
}
