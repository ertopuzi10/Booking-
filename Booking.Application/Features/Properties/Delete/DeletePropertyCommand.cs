using MediatR;

namespace Booking.Application.Features.Properties.Delete
{
    public record DeletePropertyCommand(int Id) : IRequest<Unit>;
}