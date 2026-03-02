using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Domain.Entities;

namespace Booking.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<Users> UsersQuery { get; }
        IQueryable<Roles> RolesQuery { get; }
        IQueryable<UserRoles> UserRolesQuery { get; }

        void Add<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
