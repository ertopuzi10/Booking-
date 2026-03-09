namespace Booking.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Application.Abstractions;
    using Booking.Domain.Entities;
    using Booking.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class PropertyAvailabilityRepository : IPropertyAvailabilityRepository
    {
        private readonly BookingDbContext _context;

        public PropertyAvailabilityRepository(BookingDbContext context)
        {
            _context = context;
        }

        public Task<List<PropertyAvailability>> GetByPropertyAndMonthAsync(int propertyId, int year, int month, CancellationToken cancellationToken)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            return _context.PropertyAvailabilities
                .Where(a => a.PropertyId == propertyId
                    && a.BlockedDate >= monthStart
                    && a.BlockedDate < monthEnd)
                .ToListAsync(cancellationToken);
        }

        public Task<List<PropertyAvailability>> GetBlockedDatesAsync(int propertyId, IEnumerable<DateTime> dates, CancellationToken cancellationToken)
        {
            var dateDates = dates.Select(d => d.Date).ToList();
            return _context.PropertyAvailabilities
                .Where(a => a.PropertyId == propertyId && dateDates.Contains(a.BlockedDate))
                .ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<PropertyAvailability> availabilities, CancellationToken cancellationToken)
        {
            await _context.PropertyAvailabilities.AddRangeAsync(availabilities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveRangeAsync(IEnumerable<PropertyAvailability> availabilities, CancellationToken cancellationToken)
        {
            _context.PropertyAvailabilities.RemoveRange(availabilities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddOrUpdateSeasonalPriceAsync(PropertySeasonalPrice seasonalPrice, CancellationToken cancellationToken)
        {
            // Check for existing overlapping seasonal price for same property
            var existing = await _context.PropertySeasonalPrices
                .FirstOrDefaultAsync(sp => sp.PropertyId == seasonalPrice.PropertyId
                    && sp.StartDate == seasonalPrice.StartDate
                    && sp.EndDate == seasonalPrice.EndDate, cancellationToken);

            if (existing != null)
            {
                existing.PricePerNight = seasonalPrice.PricePerNight;
                _context.PropertySeasonalPrices.Update(existing);
            }
            else
            {
                await _context.PropertySeasonalPrices.AddAsync(seasonalPrice, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
