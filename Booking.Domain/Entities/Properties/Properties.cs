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

        // Pricing rules (stored per property so each host can configure their own fees)
        public decimal CleaningFee { get; set; } = 0m;
        public decimal ExtraGuestFeePerNight { get; set; } = 0m;
        public int BaseGuestsIncluded { get; set; } = 2;
        public decimal ServiceFeePercent { get; set; } = 0.10m;
        public decimal TaxPercent { get; set; } = 0.085m;

        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;

        // Stored average rating — updated atomically on every review submission
        public decimal? AverageRating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastBookedOnUtc { get; set; }

        // Navigation properties
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
        public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public ICollection<PropertyAmenity> Amenities { get; set; } = new List<PropertyAmenity>();
        public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
        public ICollection<PropertyAvailability> BlockedDates { get; set; } = new List<PropertyAvailability>();
        public ICollection<PropertySeasonalPrice> SeasonalPrices { get; set; } = new List<PropertySeasonalPrice>();
    }
}