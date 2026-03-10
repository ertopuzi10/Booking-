using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Domain.Entities;

namespace Booking.Application.Abstractions
{
    public interface IBookingRepository
    {
        Task AddAsync(Bookings booking, CancellationToken cancellationToken);
        Task<Bookings?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Bookings>> GetByUserAsync(int userId, string? status, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
