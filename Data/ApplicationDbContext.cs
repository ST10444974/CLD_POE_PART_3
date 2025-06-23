using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Venue_Booking_System.Models;

namespace Venue_Booking_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingsDetailsView> BookingsDetailsView { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the view as keyless
            modelBuilder.Entity<BookingsDetailsView>(eb =>
            {
                eb.HasNoKey();
                eb.ToView("BookingsDetailsView");
            });
        }
    }
}
