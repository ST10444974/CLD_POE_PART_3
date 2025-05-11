using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Venue_Booking_System.Migrations
{
    /// <inheritdoc />
    public partial class BookingStartEndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingDate",
                table: "Bookings",
                newName: "BookingStartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingEndDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingEndDate",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "BookingStartDate",
                table: "Bookings",
                newName: "BookingDate");
        }
    }
}
