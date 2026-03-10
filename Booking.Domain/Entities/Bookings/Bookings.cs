using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities
{
    public class Bookings
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public Properties Property { get; set; } = null!;

        public int GuestId { get; set; }
        public Users Guest { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestCount { get; set; }

        public decimal PriceForPeriod { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal AmenitiesUpCharge { get; set; }  // Extra guest fee
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalPrice { get; set; }

        public string BookingStatus { get; set; } = "Pending";
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedOnUtc { get; set; }
        public DateTime? RejectedOnUtc { get; set; }
        public DateTime? CompletedOnUtc { get; set; }
        public DateTime? CancelledOnUtc { get; set; }
        public DateTime? ExpiredOnUtc { get; set; }

        // Navigation property
        public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}
