using System.ComponentModel.DataAnnotations;

namespace Venue_Booking_System.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        [Required]
        public string EventName { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Event Start date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EventStartDate { get; set; }

        [Display(Name = "Event End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EventEndDate { get; set; }



    }
}
