using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities
{
    public class Properties
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }
        public Users Owner { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;

        public int AddressId { get; set; }
        public Addresses Address { get; set; } = null!;

        public int MaxGuests { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastBookedOnUtc { get; set; }

        // Navigation properties
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
    }
}
