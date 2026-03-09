using MediatR;
using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Availability.BlockDates
{
    public class BlockDatesCommand : IRequest<Unit>
    {
        public int PropertyId { get; set; }
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public string? Reason { get; set; }
    }
}
