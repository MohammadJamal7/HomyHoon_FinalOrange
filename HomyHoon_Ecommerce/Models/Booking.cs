using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomyHoon.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [Required]
        public string BookingStatus { get; set; }  // E.g., "Pending", "Confirmed", "Canceled"

        // Foreign key for User
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Foreign key for Apartment
        public int ApartmentId { get; set; }
        [ForeignKey("ApartmentId")]
        public virtual Apartment Apartment { get; set; }
    }
}
