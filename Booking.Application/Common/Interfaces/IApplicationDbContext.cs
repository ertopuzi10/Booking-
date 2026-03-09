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
        IQueryable<Properties> PropertiesQuery { get; }
        IQueryable<PropertyAmenity> PropertyAmenitiesQuery { get; }
        IQueryable<PropertyPhoto> PropertyPhotosQuery { get; }
        IQueryable<PropertyAvailability> PropertyAvailabilitiesQuery { get; }
        IQueryable<PropertySeasonalPrice> SeasonalPricesQuery { get; }
        IQueryable<Bookings> BookingsQuery { get; }
        IQueryable<Reviews> ReviewsQuery { get; }

        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
