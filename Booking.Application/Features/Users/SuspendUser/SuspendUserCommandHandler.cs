using Booking.Application.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.SuspendUser
{
    public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public SuspendUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"User with id {request.UserId} not found.");

            user.IsSuspended = !user.IsSuspended;
            user.LastModifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
