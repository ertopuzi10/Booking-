using MediatR;

namespace Booking.Application.Features.Properties.GetById
{
    public record GetPropertyByIdQuery(int Id) : IRequest<GetPropertyByIdDto>;
}