using Booking.Application.Abstractions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.GetById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserByIdDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetWithRolesAsync(request.Id, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"User with id {request.Id} not found.");

            var roles = user.UserRoles
                .Select(ur => ur.Role?.Name ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            return new GetUserByIdDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                IsActive = user.IsActive,
                IsSuspended = user.IsSuspended,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };
        }
    }
}
