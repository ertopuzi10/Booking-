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
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public RegisterUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required");

            // Check existing user
            var exists = _context.UsersQuery.Any(u => u.Email == request.Email);
            if (exists)
                throw new InvalidOperationException("Email already registered");

            // ckeck if roles exist
            var userRole = _context.RolesQuery.FirstOrDefault(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Roles { Name = "User", Description = "Default user role", IsDefault = true };
                _context.Add(userRole);
            }

            var adminRole = _context.RolesQuery.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Roles { Name = "Admin", Description = "Administrator role", IsDefault = false };
                _context.Add(adminRole);
            }

            string Hash(string input)
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }

            var user = new global::Booking.Domain.Entities.Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _context.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            var roleToAssign = request.IsAdmin ? adminRole : userRole;
            var trackedRole = _context.RolesQuery.FirstOrDefault(r => r.Name == roleToAssign.Name);
            if (trackedRole == null)
            {
                trackedRole = roleToAssign;
                _context.Add(trackedRole);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var userRoleEntry = new UserRoles
            {
                UserId = user.Id,
                RoleId = trackedRole.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.Add(userRoleEntry);
            await _context.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
