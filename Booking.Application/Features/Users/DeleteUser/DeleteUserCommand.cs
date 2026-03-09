using MediatR;

namespace Booking.Application.Features.Users.DeleteUser
{
    public record DeleteUserCommand(int UserId) : IRequest<Unit>;
}
