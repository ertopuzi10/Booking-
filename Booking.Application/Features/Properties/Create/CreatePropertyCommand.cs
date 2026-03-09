using MediatR;
using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Properties.Create
{
    public class CreatePropertyCommand : IRequest<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;
        public int AddressId { get; set; }
        public int MaxGuests { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Rules { get; set; }
        public int MinStayNights { get; set; } = 1;
        public int? MaxStayNights { get; set; }
        public List<string> AmenityNames { get; set; } = new List<string>();
        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}
