using System.Collections.Generic;
using MediatR;

namespace Booking.Application.Features.Bookings.Queries.GetBookingHistory
{
    public class GetBookingHistoryQuery : IRequest<List<GetBookingHistoryDto>>
    {
        public string? Status { get; set; }
    }
}
