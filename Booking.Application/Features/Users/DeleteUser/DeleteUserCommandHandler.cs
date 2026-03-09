using Booking.Application.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"User with id {request.UserId} not found.");

            user.IsActive = false;
            user.LastModifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
