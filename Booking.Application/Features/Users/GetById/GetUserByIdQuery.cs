using MediatR;

namespace Booking.Application.Features.Users.GetById
{
    public record GetUserByIdQuery(int Id) : IRequest<GetUserByIdDto>;
}
