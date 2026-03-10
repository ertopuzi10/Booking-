using System;

namespace Booking.Application.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int GuestId { get; set; }
        public string GuestFullName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int GuestCount { get; set; }
        public decimal PriceForPeriod { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal ExtraGuestFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedOnUtc { get; set; }
        public DateTime? RejectedOnUtc { get; set; }
        public DateTime? CancelledOnUtc { get; set; }
        public DateTime? CompletedOnUtc { get; set; }
        public DateTime? ExpiredOnUtc { get; set; }
    }
}
