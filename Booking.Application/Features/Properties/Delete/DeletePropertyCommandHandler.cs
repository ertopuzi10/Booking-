using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Delete
{
    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;

        public DeletePropertyCommandHandler(IPropertyRepository repository, ICurrentUserService currentUserService, IApplicationDbContext context)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<Unit> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new System.UnauthorizedAccessException("You must be logged in to delete a property.");

            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _context.UserRolesQuery
                .Any(ur => ur.UserId == currentUserId && ur.Role.Name == "Admin");

            if (property.OwnerId != currentUserId && !isAdmin)
                throw new System.UnauthorizedAccessException("You are not authorized to delete this property.");

            await _repository.DeleteAsync(property, cancellationToken);

            return Unit.Value;
        }
    }
}
