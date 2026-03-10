using System;
using MediatR;

namespace Booking.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<int>
    {
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestCount { get; set; }
    }
}
