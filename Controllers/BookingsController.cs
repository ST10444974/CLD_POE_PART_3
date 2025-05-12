using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Venue_Booking_System.Data;
using Venue_Booking_System.Models;

namespace Venue_Booking_System.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings.Include(b => b.Venue).Include(b => b.Event);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingStartDate,BookingEndDate")] Booking booking)
        {
            // Auto-set booking dates to match event dates
            var selectedEvent = await _context.Events.FindAsync(booking.EventId);
            if (selectedEvent != null)
            {
                booking.BookingStartDate = selectedEvent.EventStartDate.Date;
                booking.BookingEndDate = selectedEvent.EventEndDate.Date;
            }

            // Double booking validation
            bool hasConflict = await _context.Bookings
                .AnyAsync(b => b.VenueId == booking.VenueId &&
                    b.BookingId != booking.BookingId && // Exclude current booking during edits
                    (b.BookingStartDate.Date <= booking.BookingEndDate.Date &&
                     b.BookingEndDate.Date >= booking.BookingStartDate.Date));

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This venue is already booked for the selected dates.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingStartDate,BookingEndDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            // Fetch the event to auto-set dates (if needed)
            var selectedEvent = await _context.Events.FindAsync(booking.EventId);
            if (selectedEvent != null)
            {
                booking.BookingStartDate = selectedEvent.EventStartDate.Date;
                booking.BookingEndDate = selectedEvent.EventEndDate.Date;
            }

            // Check for overlapping bookings (exclude current booking)
            bool hasConflict = await _context.Bookings
                .AnyAsync(b => b.VenueId == booking.VenueId &&
                    b.BookingId != booking.BookingId && // Exclude current booking
                    (b.BookingStartDate.Date <= booking.BookingEndDate.Date &&
                     b.BookingEndDate.Date >= booking.BookingStartDate.Date));

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This venue is already booked for the selected dates.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
