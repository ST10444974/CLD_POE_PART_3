using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Venue_Booking_System.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Display(Name = "Venue Name")]
        [Required]
        public string? VenueName { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        
        public string? ImageUrl { get; set; }

        [Display(Name = "Available?")]
        public bool IsAvailable { get; set; } = true; // Default to available

        public EventType? EventType { get; set; }

    }
}
