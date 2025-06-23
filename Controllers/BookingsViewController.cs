using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Venue_Booking_System.Data;
using Venue_Booking_System.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Venue_Booking_System.Controllers
{
    public class BookingsViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchString,
            int? eventType,
            DateTime? startDate,
            DateTime? endDate,
            bool availableOnly = false)
        {
            // Start with joined query
            var baseQuery = from booking in _context.Bookings
                            join venue in _context.Venues on booking.VenueId equals venue.VenueId
                            join evt in _context.Events on booking.EventId equals evt.EventId
                            join eventTypeObj in _context.EventTypes on evt.EventTypeId equals eventTypeObj.EventTypeId
                            select new
                            {
                                Booking = booking,
                                Venue = venue,
                                Event = evt,
                                EventType = eventTypeObj
                            };

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                baseQuery = baseQuery.Where(x =>
                    x.Booking.BookingId.ToString().Contains(searchString) ||
                    x.Event.EventName.Contains(searchString));
            }

            if (eventType.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Event.EventTypeId == eventType.Value);
            }

            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Booking.BookingStartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Booking.BookingEndDate <= endDate.Value);
            }

            if (availableOnly)
            {
                baseQuery = baseQuery.Where(x => x.Venue.IsAvailable);
            }

            // Project to view model
            var results = await baseQuery.Select(x => new BookingsDetailsView
            {
                BookingId = x.Booking.BookingId,
                BookingStartDate = x.Booking.BookingStartDate,
                BookingEndDate = x.Booking.BookingEndDate,
                EventName = x.Event.EventName,
                EventDescription = x.Event.Description,
                EventStartDate = x.Event.EventStartDate,
                EventEndDate = x.Event.EventEndDate,
                VenueName = x.Venue.VenueName,
                VenueLocation = x.Venue.Location,
                VenueCapacity = x.Venue.Capacity,
                EventType = x.EventType.TypeName,
                VenueAvailability = x.Venue.IsAvailable
            }).ToListAsync();

            // Populate EventType dropdown
            ViewBag.EventTypes = await _context.EventTypes.ToListAsync();

            return View(results);
        }
    }

}