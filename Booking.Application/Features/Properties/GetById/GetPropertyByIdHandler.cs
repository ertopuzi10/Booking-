using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.GetById
{
    public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, GetPropertyByIdDto>
    {
        private readonly IApplicationDbContext _context;

        public GetPropertyByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetPropertyByIdDto> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var property = await _context.PropertiesQuery
                .Include(p => p.Address)
                .Include(p => p.Amenities)
                .Include(p => p.Photos)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Guest)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (property == null)
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");

            var amenityNames = property.Amenities.Select(a => a.Name).ToList();
            var photoUrls = property.Photos.OrderBy(p => p.DisplayOrder).Select(p => p.Url).ToList();
            var reviews = property.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PropertyReviewDto
                {
                    Id = r.Id,
                    GuestFullName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();

            return new GetPropertyByIdDto
            {
                Id = property.Id,
                OwnerId = property.OwnerId,
                Name = property.Name,
                Description = property.Description,
                PropertyType = property.PropertyType,
                AddressId = property.AddressId,
                MaxGuests = property.MaxGuests,
                CheckInTime = property.CheckInTime,
                CheckOutTime = property.CheckOutTime,
                PricePerNight = property.PricePerNight,
                Rules = property.Rules,
                MinStayNights = property.MinStayNights,
                MaxStayNights = property.MaxStayNights,
                IsActive = property.IsActive,
                IsApproved = property.IsApproved,
                AddressCity = property.Address?.City,
                AddressCountry = property.Address?.Country,
                AddressStreet = property.Address?.Street,
                AmenityNames = amenityNames,
                PhotoUrls = photoUrls,
                AverageRating = property.AverageRating,
                Reviews = reviews,
                CleaningFee = property.CleaningFee,
                ExtraGuestFeePerNight = property.ExtraGuestFeePerNight,
                BaseGuestsIncluded = property.BaseGuestsIncluded,
                ServiceFeePercent = property.ServiceFeePercent,
                TaxPercent = property.TaxPercent
            };
        }
    }
}
