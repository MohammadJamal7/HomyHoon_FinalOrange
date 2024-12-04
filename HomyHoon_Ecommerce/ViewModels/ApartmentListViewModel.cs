using HomyHoon.Models;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class ApartmentListViewModel
    {
        public List<Apartment> Apartments { get; set; }
        public List<Category> categories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string CategoryFilter { get; set; }
        public int BedroomsFilter { get; set; }
        public string CityFilter { get; set; }
        public string FurnishedFilter { get; set; }
        public int BathroomsFilter { get; set; }
        public int BedsFilter { get; set; }
        public bool IncludeReserved { get; set; }


    }
}
