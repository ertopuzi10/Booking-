using MediatR;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Application.Features.Users.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthManager _authManager;

        public RegisterUserCommandHandler(IApplicationDbContext context, IAuthManager authManager)
        {
            _context = context;
            _authManager = authManager;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required");
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required");
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("Last name is required");

            var emailExists = _context.UsersQuery.Any(u => u.Email == request.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered");

            var usernameExists = _context.UsersQuery.Any(u => u.Username == request.Username);
            if (usernameExists)
                throw new InvalidOperationException("Username already taken");

            string Hash(string input)
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }

            var user = new Booking.Domain.Entities.Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Password = Hash(request.Password),
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                IsSuspended = false,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _context.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Assign default "Guest" role
            var guestRole = _context.RolesQuery.FirstOrDefault(r => r.Name == "Guest");
            if (guestRole == null)
                throw new InvalidOperationException("Guest role not found. Please ensure roles are seeded.");

            var userRoleEntry = new UserRoles
            {
                UserId = user.Id,
                RoleId = guestRole.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.Add(userRoleEntry);
            await _context.SaveChangesAsync(cancellationToken);

            // Generate JWT token
            var token = _authManager.GenerateJwtToken(user.Id, user.Email, user.FirstName, user.LastName, new[] { guestRole.Name });

            return new RegisterUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = token
            };
        }
    }
}
