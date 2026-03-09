using MediatR;

namespace Booking.Application.Features.Users.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Unit>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
