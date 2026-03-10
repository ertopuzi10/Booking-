using MediatR;

namespace Booking.Application.Features.Bookings.Commands.ConfirmBooking
{
    public class ConfirmBookingCommand : IRequest<Unit>
    {
        public int BookingId { get; set; }
    }
}
