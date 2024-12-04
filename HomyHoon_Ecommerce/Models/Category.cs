using System.ComponentModel.DataAnnotations;

namespace HomyHoon.Models
{
    public class Category
    {

        [Key]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; } 

            public string Description { get; set; }
            public string ? ImagePath { get; set; }
            // Navigation property for related apartments
            public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
        

    }
}
