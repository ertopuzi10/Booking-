using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Availability.BlockDates
{
    public class BlockDatesCommandHandler : IRequestHandler<BlockDatesCommand, Unit>
    {
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;

        public BlockDatesCommandHandler(
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

        public async Task<Unit> Handle(BlockDatesCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to block dates.");

            if (request.Dates == null || !request.Dates.Any())
                throw new ArgumentException("At least one date must be provided.");

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.PropertyId} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _context.UserRolesQuery
                .Any(ur => ur.UserId == currentUserId && ur.Role.Name == "Admin");

            if (property.OwnerId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to block dates for this property.");

            var availabilities = request.Dates
                .Select(d => new PropertyAvailability
                {
                    PropertyId = request.PropertyId,
                    BlockedDate = d.Date,
                    Reason = request.Reason
                })
                .ToList();

            await _availabilityRepository.AddRangeAsync(availabilities, cancellationToken);

            return Unit.Value;
        }
    }
}
