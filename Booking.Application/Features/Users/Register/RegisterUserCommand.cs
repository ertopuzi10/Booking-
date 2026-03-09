using MediatR;

namespace Booking.Application.Features.Users.Register
{
    public class RegisterUserCommand : IRequest<RegisterUserResponse>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }    
    }

    public class RegisterUserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}

