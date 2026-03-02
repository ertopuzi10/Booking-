using System;

namespace Booking.Domain.Entities
{
    public class OwnerProfiles
    {
        public int UserId { get; set; }

        public string IdentityCardNumber { get; set; } = null!;
        public bool VerificationStatus { get; set; } = false;
        public string? BusinessName { get; set; }
        public string? CreditCard { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Users User { get; set; } = null!;
    }
}
