namespace Booking.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Application.Abstractions;
    using Booking.Domain.Entities;
    using Booking.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class PropertyRepository : IPropertyRepository
    {
        private readonly BookingDbContext _context;

        public PropertyRepository(BookingDbContext context)
        {
            _context = context;
        }

        public Task<List<Properties>> GetAllAsync(CancellationToken cancellationToken) =>
            _context.Properties.AsNoTracking().ToListAsync(cancellationToken);

        public Task<Properties?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            _context.Properties.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task AddAsync(Properties property, CancellationToken cancellationToken)
        {
            await _context.Properties.AddAsync(property, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Properties property, CancellationToken cancellationToken)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Properties property, CancellationToken cancellationToken)
        {
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
