using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Availability.UnblockDates
{
    public class UnblockDatesCommandHandler : IRequestHandler<UnblockDatesCommand, Unit>
    {
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;

        public UnblockDatesCommandHandler(
            IPropertyAvailabilityRepository availabilityRepository,
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            IApplicationDbContext context)
        {
            _availabilityRepository = availabilityRepository;
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<Unit> Handle(UnblockDatesCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to unblock dates.");

            if (request.Dates == null || !request.Dates.Any())
                throw new ArgumentException("At least one date must be provided.");

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.PropertyId} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _context.UserRolesQuery
                .Any(ur => ur.UserId == currentUserId && ur.Role.Name == "Admin");

            if (property.OwnerId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to unblock dates for this property.");

            var datesToUnblock = request.Dates.Select(d => d.Date).ToList();
            var existingBlocked = await _availabilityRepository.GetBlockedDatesAsync(request.PropertyId, datesToUnblock, cancellationToken);

            await _availabilityRepository.RemoveRangeAsync(existingBlocked, cancellationToken);

            return Unit.Value;
        }
    }
}
