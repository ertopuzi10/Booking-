using MediatR;

namespace Booking.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<CancelBookingResult>
    {
        public int BookingId { get; set; }
        public string? Reason { get; set; }
    }

    public class CancelBookingResult
    {
        public string PolicyDescription { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
    }
}
