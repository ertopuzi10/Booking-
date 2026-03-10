using System;

namespace Booking.Application.Features.Bookings.Queries.GetBookingHistory
{
    public class GetBookingHistoryDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsUpcoming { get; set; }
    }
}
