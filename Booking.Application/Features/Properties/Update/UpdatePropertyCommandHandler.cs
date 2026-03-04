using Booking.Application.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Update
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;

        public UpdatePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (property == null)
            {
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");
            }

            property.Name = request.Name;
            property.Description = request.Description;
            property.PropertyType = request.PropertyType;
            property.MaxGuests = request.MaxGuests;
            property.IsActive = request.IsActive;
            property.IsApproved = request.IsApproved;
            property.LastModifiedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(property, cancellationToken);

            return Unit.Value;
        }
    }
}

