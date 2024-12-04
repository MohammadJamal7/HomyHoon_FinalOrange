using HomyHoon.Models;
using System.ComponentModel.DataAnnotations;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class UpdateApartmentVM
    {
        public int Id { get; set; } 

        [Required]
        public string Name { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Rooms must be at least 1.")]
        public int Rooms { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Beds must be at least 1.")]
        public int Beds { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Baths must be at least 1.")]
        public int Baths { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Maximum guests must be at least 1.")]
        public int MaxGuests { get; set; }

        public string Description { get; set; }

        public bool IsFrunshised { get; set; }

        // Features
        public bool Kitchen { get; set; }
        public bool FirstAid { get; set; }
        public bool TV { get; set; }
        public bool Wifi { get; set; }
        public bool Parking { get; set; }
        public bool AirConditioning { get; set; }
        public bool Heating { get; set; }

        
        public string MainImage { get; set; } 
        public ICollection<ApartmentImage> Images { get; set; }

        
        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

       
        public List<Category> Categories { get; set; } 
    
}
}
