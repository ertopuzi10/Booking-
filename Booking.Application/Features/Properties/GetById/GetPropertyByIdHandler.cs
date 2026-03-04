using Booking.Application.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.GetById
{
    public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, GetPropertyByIdDto>
    {
        private readonly IPropertyRepository _repository;

        public GetPropertyByIdHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPropertyByIdDto> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (property == null)
            {
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");
            }

            return new GetPropertyByIdDto
            {
                Id = property.Id,
                OwnerId = property.OwnerId,
                Name = property.Name,
                Description = property.Description,
                PropertyType = property.PropertyType,
                AddressId = property.AddressId,
                MaxGuests = property.MaxGuests,
                IsActive = property.IsActive,
                IsApproved = property.IsApproved
            };
        }
    }
}

