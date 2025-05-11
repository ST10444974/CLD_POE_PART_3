// Controllers/BookingsViewController.cs
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

        // GET: BookingsView
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Select(b => new BookingsDetailsView
                {
                    BookingId = b.BookingId,
                    BookingStartDate = b.BookingStartDate,
                    BookingEndDate = b.BookingEndDate,
                    EventName = b.Event.EventName,
                    EventDescription = b.Event.Description,
                    EventStartDate = b.Event.EventStartDate,
                    EventEndDate = b.Event.EventEndDate,
                    VenueName = b.Venue.VenueName,
                    VenueLocation = b.Venue.Location,
                    VenueCapacity = b.Venue.Capacity
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int bookingId))
                {
                    query = query.Where(b => b.BookingId == bookingId);
                }
                else
                {
                    query = query.Where(b => b.EventName.ToLower().Contains(searchString.ToLower()));
                }
            }

            return View(await query.ToListAsync());
        }
    }
}