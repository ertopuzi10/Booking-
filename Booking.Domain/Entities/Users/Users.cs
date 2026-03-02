using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
        public ICollection<Properties> Properties { get; set; } = new List<Properties>();
        public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
        public OwnerProfiles? OwnerProfile { get; set; }
    }
}
