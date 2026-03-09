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

        public decimal PricePerNight { get; set; }
        public string? Rules { get; set; }
        public int MinStayNights { get; set; } = 1;
        public int? MaxStayNights { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastBookedOnUtc { get; set; }

        // Navigation properties
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
        public ICollection<PropertyAmenity> Amenities { get; set; } = new List<PropertyAmenity>();
        public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
        public ICollection<PropertyAvailability> BlockedDates { get; set; } = new List<PropertyAvailability>();
        public ICollection<PropertySeasonalPrice> SeasonalPrices { get; set; } = new List<PropertySeasonalPrice>();
    }
}