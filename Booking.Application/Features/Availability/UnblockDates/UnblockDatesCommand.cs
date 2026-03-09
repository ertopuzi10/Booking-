using MediatR;
using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Availability.UnblockDates
{
    public class UnblockDatesCommand : IRequest<Unit>
    {
        public int PropertyId { get; set; }
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
    }
}
