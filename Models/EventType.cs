using System.ComponentModel.DataAnnotations;

namespace Venue_Booking_System.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        public string TypeName { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
