using MediatR;
using System.Collections.Generic;

namespace Booking.Application.Features.Users.Login
{
    public class LoginUserCommand : IRequest<LoginUserResponse>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginUserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
