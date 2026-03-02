using System;
using System.Collections.Generic;

namespace Booking.Domain.Entities
{
    public class Addresses
    {
        public int Id { get; set; }

        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string PostalCode { get; set; } = null!;

        // Navigation property
        public ICollection<Properties> Properties { get; set; } = new List<Properties>();
    }
}
