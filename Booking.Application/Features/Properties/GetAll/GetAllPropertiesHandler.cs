using Booking.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.GetAll
{
    public class GetAllPropertiesHandler : IRequestHandler<GetAllPropertiesQuery, List<GetAllPropertiesDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPropertiesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<GetAllPropertiesDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = _context.PropertiesQuery
                .Select(p => new GetAllPropertiesDto
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Name = p.Name,
                    Description = p.Description,
                    PropertyType = p.PropertyType,
                    AddressId = p.AddressId,
                    MaxGuests = p.MaxGuests,
                    IsActive = p.IsActive,
                    IsApproved = p.IsApproved,
                    PricePerNight = p.PricePerNight,
                    AverageRating = p.Bookings
                        .SelectMany(b => b.Reviews)
                        .Any()
                        ? (double?)p.Bookings
                            .SelectMany(b => b.Reviews)
                            .Average(r => r.Rating)
                        : null
                })
                .ToList();

            return Task.FromResult(properties);
        }
    }
}
