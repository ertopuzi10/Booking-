using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Availability.SetSeasonalPrice
{
    public class SetSeasonalPriceCommandHandler : IRequestHandler<SetSeasonalPriceCommand, Unit>
    {
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;

        public SetSeasonalPriceCommandHandler(
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

        public async Task<Unit> Handle(SetSeasonalPriceCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to set seasonal prices.");

            if (request.StartDate >= request.EndDate)
                throw new ArgumentException("StartDate must be before EndDate.");
            if (request.PricePerNight <= 0)
                throw new ArgumentException("PricePerNight must be greater than zero.");

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.PropertyId} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _context.UserRolesQuery
                .Any(ur => ur.UserId == currentUserId && ur.Role.Name == "Admin");

            if (property.OwnerId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to set seasonal prices for this property.");

            var seasonalPrice = new PropertySeasonalPrice
            {
                PropertyId = request.PropertyId,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                PricePerNight = request.PricePerNight
            };

            await _availabilityRepository.AddOrUpdateSeasonalPriceAsync(seasonalPrice, cancellationToken);

            return Unit.Value;
        }
    }
}
