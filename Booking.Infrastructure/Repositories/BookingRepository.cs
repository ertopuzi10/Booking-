using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bookings booking, CancellationToken cancellationToken)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Bookings?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<List<Bookings>> GetByUserAsync(int userId, string? status, CancellationToken cancellationToken)
        {
            var query = _context.Bookings
                .Include(b => b.Property)
                .AsNoTracking()
                .Where(b => b.GuestId == userId || b.Property.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(b => b.BookingStatus == status);

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
