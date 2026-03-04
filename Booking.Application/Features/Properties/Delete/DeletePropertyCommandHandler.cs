using Booking.Application.Abstractions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Delete
{
    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _repository;

        public DeletePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (property == null)
            {
                throw new KeyNotFoundException($"Property with id {request.Id} not found.");
            }

            await _repository.DeleteAsync(property, cancellationToken);

            return Unit.Value;
        }
    }
}

