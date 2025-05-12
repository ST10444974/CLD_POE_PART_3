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
            // Query directly from the SQL view
            var query = _context.BookingsDetailsView.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int bookingId))
                {
                    query = query.Where(b => b.BookingId == bookingId);
                }
                else
                {
                    query = query.Where(b => b.EventName.Contains(searchString));
                }
            }

            // Project to your existing BookingsDetailsView model
            var result = await query.Select(b => new BookingsDetailsView
            {
                BookingId = b.BookingId,
                BookingStartDate = b.BookingStartDate,
                BookingEndDate = b.BookingEndDate,
                EventName = b.EventName,
                EventDescription = b.EventDescription,
                EventStartDate = b.EventStartDate,
                EventEndDate = b.EventEndDate,
                VenueName = b.VenueName,
                VenueLocation = b.VenueLocation,
                VenueCapacity = b.VenueCapacity
            }).ToListAsync();

            return View(result);
        }
    }
}