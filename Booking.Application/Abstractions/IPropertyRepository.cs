namespace Booking.Application.Abstractions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Application.Features.Properties.Search;
    using Booking.Domain.Entities;

    public interface IPropertyRepository
    {
        Task<List<Properties>> GetAllAsync(CancellationToken cancellationToken);
        Task<Properties?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Properties property, CancellationToken cancellationToken);
        Task UpdateAsync(Properties property, CancellationToken cancellationToken);
        Task DeleteAsync(Properties property, CancellationToken cancellationToken);
        Task<List<SearchPropertiesDto>> SearchAsync(SearchPropertiesQuery query, CancellationToken cancellationToken);
    }
}
