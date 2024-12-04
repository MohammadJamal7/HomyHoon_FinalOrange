using HomyHoon.Models;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class AddApartmentVM
    {
        public Apartment ? Apartment { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
