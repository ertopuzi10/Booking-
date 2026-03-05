using System;

namespace Booking.Domain.Entities
{
    public class Reviews
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Bookings Bookings { get; set; } = null!;

        public int GuestId { get; set; }
        public Users Guest { get; set; } = null!;

        public int Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
