using MediatR;

namespace Booking.Application.Features.Users.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Unit>
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
