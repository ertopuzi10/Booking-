using Booking.Application.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Create
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly IPropertyRepository _repository;

        public CreatePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = new Domain.Entities.Properties
            {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Description = request.Description,
                PropertyType = request.PropertyType,
                AddressId = request.AddressId,
                MaxGuests = request.MaxGuests,
                IsActive = true,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(property, cancellationToken);

            return property.Id;
        }
    }
}

