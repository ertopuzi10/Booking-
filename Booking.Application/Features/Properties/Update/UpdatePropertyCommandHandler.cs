using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Update
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;

        public UpdatePropertyCommandHandler(IPropertyRepository repository, ICurrentUserService currentUserService, IApplicationDbContext context)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<Unit> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to update a property.");

            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _context.UserRolesQuery
                .Any(ur => ur.UserId == currentUserId && ur.Role.Name == "Admin");

            if (property.OwnerId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to update this property.");

            property.Name = request.Name;
            property.Description = request.Description;
            property.PropertyType = request.PropertyType;
            property.MaxGuests = request.MaxGuests;
            property.CheckInTime = request.CheckInTime;
            property.CheckOutTime = request.CheckOutTime;
            property.PricePerNight = request.PricePerNight;
            property.Rules = request.Rules;
            property.MinStayNights = request.MinStayNights;
            property.MaxStayNights = request.MaxStayNights;
            property.IsActive = request.IsActive;
            property.IsApproved = request.IsApproved;
            property.CleaningFee = request.CleaningFee;
            property.ExtraGuestFeePerNight = request.ExtraGuestFeePerNight;
            property.BaseGuestsIncluded = request.BaseGuestsIncluded;
            property.ServiceFeePercent = request.ServiceFeePercent;
            property.TaxPercent = request.TaxPercent;
            property.LastModifiedAt = DateTime.UtcNow;

            // Replace amenities
            property.Amenities.Clear();
            foreach (var amenityName in request.AmenityNames)
            {
                if (!string.IsNullOrWhiteSpace(amenityName))
                {
                    property.Amenities.Add(new PropertyAmenity { PropertyId = property.Id, Name = amenityName });
                }
            }

            // Replace photos
            property.Photos.Clear();
            int displayOrder = 1;
            foreach (var photoUrl in request.PhotoUrls)
            {
                if (!string.IsNullOrWhiteSpace(photoUrl))
                {
                    property.Photos.Add(new PropertyPhoto { PropertyId = property.Id, Url = photoUrl, DisplayOrder = displayOrder++ });
                }
            }

            await _repository.UpdateAsync(property, cancellationToken);

            return Unit.Value;
        }
    }
}
