namespace Venue_Booking_System.Models
{
    public class BookingsDetailsView
    {
        public int BookingId { get; set; }
        public DateTime BookingStartDate { get; set; }
        public DateTime BookingEndDate { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
    }
}