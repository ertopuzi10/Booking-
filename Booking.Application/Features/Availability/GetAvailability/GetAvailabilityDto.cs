using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Availability.GetAvailability
{
    public class GetAvailabilityDto
    {
        public int PropertyId { get; init; }
        public int Year { get; init; }
        public int Month { get; init; }
        public List<DateTime> BlockedDates { get; init; } = new List<DateTime>();
        public List<BookedPeriodDto> BookedPeriods { get; init; } = new List<BookedPeriodDto>();
    }

    public class BookedPeriodDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string BookingStatus { get; init; } = null!;
    }
}
