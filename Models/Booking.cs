using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Venue_Booking_System.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("Event")]
        [Required]
        public int EventId { get; set; }

        [ForeignKey("Venue")]
        [Required]
        public int VenueId { get; set; }

        [Display(Name = "Booking Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingStartDate { get; set; }

        [Display(Name = "Booking End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingEndDate { get; set; }



        public Event? Event { get; set; }
        public Venue? Venue { get; set; }
    }
}
