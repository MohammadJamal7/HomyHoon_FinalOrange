using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomyHoon.Models
{
    public class Apartment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxGuests { get; set; }

        [Range(0, int.MaxValue)]
        public int Beds { get; set; }

        [Range(0, int.MaxValue)]
        public int Baths { get; set; }

        public bool IsFrunshised { get; set; }

        [Range(0, int.MaxValue)]
        public int Rooms { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }

        public string? City { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public string MainImage { get; set; } = "default-image.jpg";
        public bool IsAvailable { get; set; } = true;

        // Additional features
        public bool Wifi { get; set; } = false;
        public bool FirstAid { get; set; } = false;
        public bool AirConditioning { get; set; } = false;
        public bool Parking { get; set; } = false;
        public bool Heating { get; set; } = false;
        public bool Kitchen { get; set; } = false;
        public bool TV { get; set; } = false;

        // Foreign key and relationship to User (Owner)
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        // Navigation properties
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<ApartmentImage> Images { get; set; } = new List<ApartmentImage>();
    }
}
