using Booking.Application.Abstractions;
using Booking.Application.Services;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Create
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly IPropertyRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreatePropertyCommandHandler(IPropertyRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to create a property.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Property name is required.");
            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Property description is required.");
            if (string.IsNullOrWhiteSpace(request.PropertyType))
                throw new ArgumentException("Property type is required.");
            if (request.MaxGuests <= 0)
                throw new ArgumentException("MaxGuests must be greater than zero.");
            if (request.PricePerNight <= 0)
                throw new ArgumentException("PricePerNight must be greater than zero.");

            var property = new Domain.Entities.Properties
            {
                OwnerId = _currentUserService.UserId.Value,
                Name = request.Name,
                Description = request.Description,
                PropertyType = request.PropertyType,
                AddressId = request.AddressId,
                MaxGuests = request.MaxGuests,
                CheckInTime = request.CheckInTime,
                CheckOutTime = request.CheckOutTime,
                PricePerNight = request.PricePerNight,
                Rules = request.Rules,
                MinStayNights = request.MinStayNights,
                MaxStayNights = request.MaxStayNights,
                IsActive = true,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            foreach (var amenityName in request.AmenityNames)
            {
                if (!string.IsNullOrWhiteSpace(amenityName))
                {
                    property.Amenities.Add(new PropertyAmenity { Name = amenityName });
                }
            }

            int displayOrder = 1;
            foreach (var photoUrl in request.PhotoUrls)
            {
                if (!string.IsNullOrWhiteSpace(photoUrl))
                {
                    property.Photos.Add(new PropertyPhoto { Url = photoUrl, DisplayOrder = displayOrder++ });
                }
            }

            await _repository.AddAsync(property, cancellationToken);

            return property.Id;
        }
    }
}
