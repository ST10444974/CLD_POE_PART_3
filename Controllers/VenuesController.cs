using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Venue_Booking_System.Data;
using Venue_Booking_System.Models;
using Venue_Booking_System.Service;

namespace Venue_Booking_System.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly BlobService _blobService;

        public VenuesController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile file)
        {
            // Validate if a file is uploaded
            if (file == null)
            {
                ModelState.AddModelError("ImageUrl", "Image is required."); // Add error for ImageUrl
            }
            else
            {
                // Existing file validation and upload logic
                var fileName = file.FileName;
                var blobExists = await _blobService.BlobExistsAsync(fileName);

                if (blobExists)
                {
                    ModelState.AddModelError("ImageUrl", "Image has been uploaded previously.");
                    return View(venue);
                }

                using var stream = file.OpenReadStream();
                var blobUrl = await _blobService.uploadAsync(stream, file.FileName);
                venue.ImageUrl = blobUrl;
            }

            if (ModelState.IsValid) 
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile file)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                    if (existingVenue == null)
                    {
                        return NotFound();
                    }

                    if (file != null)
                    {
                        string newFileName = file.FileName;
                        string currentBlobName = Path.GetFileName(new Uri(existingVenue.ImageUrl).LocalPath);

                        // Check if the new filename is already used by another blob
                        if (newFileName != currentBlobName)
                        {
                            bool blobExists = await _blobService.BlobExistsAsync(newFileName);
                            if (blobExists)
                            {
                                ModelState.AddModelError("ImageUrl", "An image with this name already exists.");
                                return View(venue);
                            }

                            // Upload new image
                            using var stream = file.OpenReadStream();
                            string newBlobUrl = await _blobService.uploadAsync(stream, newFileName);

                            // Delete old image
                            await _blobService.DeleteBlobAsync(currentBlobName);

                            venue.ImageUrl = newBlobUrl;
                        }
                        else
                        {
                            // Overwrite the existing blob
                            using var stream = file.OpenReadStream();
                            await _blobService.uploadAsync(stream, newFileName); // Overwrite: true by default
                        }
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the venue.");
                    return View(venue);
                }
            }
            return View(venue);
        }

        // GET: Venues/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Delete the image from Azure Blob Storage
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    // Extract the blob name from the URL 
                    var blobName = Path.GetFileName(new Uri(venue.ImageUrl).LocalPath);
                    await _blobService.DeleteBlobAsync(blobName);
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }

    }
}