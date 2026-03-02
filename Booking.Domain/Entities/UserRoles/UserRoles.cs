using System;

namespace Booking.Domain.Entities
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Users User { get; set; } = null!;
        public Roles Role { get; set; } = null!;
    }
}
