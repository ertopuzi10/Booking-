using Booking.Application.Abstractions;
using Booking.Application.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProfileCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to update your profile.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("Last name is required.");

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.ProfileImageUrl = request.ProfileImageUrl;
            user.LastModifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
