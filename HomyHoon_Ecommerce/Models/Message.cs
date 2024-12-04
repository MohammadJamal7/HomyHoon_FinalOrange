using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HomyHoon.Models
{
    public class Message
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required, EmailAddress]
        public string SenderEmail { get; set; }

        [Required]
        public string Subject { get; set; }


        public DateTime DateSent { get; set; }
        

        // Foreign key and relationship to User (optional)
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
