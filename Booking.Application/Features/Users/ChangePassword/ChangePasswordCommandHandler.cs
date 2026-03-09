using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using MediatR;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthManager _authManager;

        public ChangePasswordCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IAuthManager authManager)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _authManager = authManager;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to change your password.");

            if (string.IsNullOrWhiteSpace(request.OldPassword))
                throw new ArgumentException("Old password is required.");
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new ArgumentException("New password is required.");

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!_authManager.VerifyPassword(request.OldPassword, user.Password))
                throw new ArgumentException("Old password is incorrect.");

            string Hash(string input)
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }

            user.Password = Hash(request.NewPassword);
            user.LastModifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
