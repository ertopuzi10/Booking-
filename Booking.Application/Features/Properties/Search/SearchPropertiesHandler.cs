using Booking.Application.Abstractions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Properties.Search
{
    public class SearchPropertiesHandler : IRequestHandler<SearchPropertiesQuery, List<SearchPropertiesDto>>
    {
        private readonly IPropertyRepository _repository;

        public SearchPropertiesHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SearchPropertiesDto>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.SearchAsync(request, cancellationToken);
        }
    }
}
