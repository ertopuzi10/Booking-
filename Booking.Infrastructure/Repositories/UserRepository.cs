namespace Booking.Infrastructure.Repositories
{
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Application.Abstractions;
    using Booking.Domain.Entities;
    using Booking.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class UserRepository : IUserRepository
    {
        private readonly BookingDbContext _context;

        public UserRepository(BookingDbContext context)
        {
            _context = context;
        }

        public Task<Users?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public Task<Users?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) =>
            _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

        public async Task AddAsync(Users user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task UpdateAsync(Users user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<Users?> GetWithRolesAsync(int id, CancellationToken cancellationToken) =>
            _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken) =>
            _context.SaveChangesAsync(cancellationToken);
    }
}
