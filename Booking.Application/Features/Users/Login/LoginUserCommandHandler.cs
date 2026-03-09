using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Common.Interfaces;

namespace Booking.Application.Features.Users.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthManager _authManager;

        public LoginUserCommandHandler(IApplicationDbContext context, IAuthManager authManager)
        {
            _context = context;
            _authManager = authManager;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required");

            var user = _context.UsersQuery.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                throw new InvalidOperationException("Invalid email or password");

            if (!_authManager.VerifyPassword(request.Password, user.Password))
                throw new InvalidOperationException("Invalid email or password");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is inactive");

            if (user.IsSuspended)
                throw new InvalidOperationException("User account is suspended");

            var roles = _context.UserRolesQuery
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .ToList();

            var token = _authManager.GenerateJwtToken(user.Id, user.Email, user.FirstName, user.LastName, roles);

            return await Task.FromResult(new LoginUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                Roles = roles
            });
        }
    }
}
