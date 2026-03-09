using MediatR;

namespace Booking.Application.Features.Availability.GetAvailability
{
    public record GetAvailabilityQuery(int PropertyId, int Year, int Month) : IRequest<GetAvailabilityDto>;
}
