namespace Booking.Application.Abstractions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Domain.Entities;

    public interface IUserRepository
    {
        Task<Users?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Users?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(Users user, CancellationToken cancellationToken);
        Task UpdateAsync(Users user, CancellationToken cancellationToken);
        Task<Users?> GetWithRolesAsync(int id, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
