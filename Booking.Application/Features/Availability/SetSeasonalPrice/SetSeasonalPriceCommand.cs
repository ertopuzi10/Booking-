using MediatR;
using System;

namespace Booking.Application.Features.Availability.SetSeasonalPrice
{
    public class SetSeasonalPriceCommand : IRequest<Unit>
    {
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
