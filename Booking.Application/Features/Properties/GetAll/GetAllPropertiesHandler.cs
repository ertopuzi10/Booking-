using Booking.Application.Abstractions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.GetAll
{
    public class GetAllPropertiesHandler : IRequestHandler<GetAllPropertiesQuery, List<GetAllPropertiesDto>>
    {
        private readonly IPropertyRepository _repository;

        public GetAllPropertiesHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAllPropertiesDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _repository.GetAllAsync(cancellationToken);

            return properties
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
                    IsApproved = p.IsApproved
                })
                .ToList();
        }
    }
}

