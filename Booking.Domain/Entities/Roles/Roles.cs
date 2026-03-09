using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities
{
    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }

        // Navigation property
        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    }
}