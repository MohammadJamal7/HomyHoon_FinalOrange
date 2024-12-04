using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomyHoon.Models
{
    public class User:IdentityUser
    {
      

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string ? ProfilePicture { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }
        public DateTime DateJoined { get; set; }

     
       

        // Navigation properties
        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>(); // If user is a host
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // If user is a renter
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>(); // If user sends messages
    }
}
