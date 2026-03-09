using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.AssignRole
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public AssignRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                throw new ArgumentException("Role name is required.");

            var user = _context.UsersQuery.FirstOrDefault(u => u.Id == request.UserId);
            if (user == null)
                throw new KeyNotFoundException($"User with id {request.UserId} not found.");

            var role = _context.RolesQuery.FirstOrDefault(r => r.Name == request.RoleName);
            if (role == null)
                throw new KeyNotFoundException($"Role '{request.RoleName}' not found.");

            var alreadyAssigned = _context.UserRolesQuery
                .Any(ur => ur.UserId == request.UserId && ur.RoleId == role.Id);

            if (alreadyAssigned)
                throw new InvalidOperationException($"User already has the role '{request.RoleName}'.");

            var userRole = new UserRoles
            {
                UserId = request.UserId,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.Add(userRole);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
