using MediatR;

namespace Booking.Application.Features.Users.AssignRole
{
    public class AssignRoleCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
