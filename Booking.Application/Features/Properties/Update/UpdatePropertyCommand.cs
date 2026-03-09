using MediatR;
using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Properties.Update
{
    public class UpdatePropertyCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;
        public int MaxGuests { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Rules { get; set; }
        public int MinStayNights { get; set; } = 1;
        public int? MaxStayNights { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public List<string> AmenityNames { get; set; } = new List<string>();
        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}
