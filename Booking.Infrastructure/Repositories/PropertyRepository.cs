namespace Booking.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Booking.Application.Abstractions;
    using Booking.Application.Features.Properties.Search;
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
            _context.Properties
                .Include(p => p.Amenities)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

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

        public Task<List<SearchPropertiesDto>> SearchAsync(SearchPropertiesQuery query, CancellationToken cancellationToken)
        {
            var q = _context.Properties
                .Include(p => p.Address)
                .Include(p => p.Amenities)
                .Include(p => p.BlockedDates)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Reviews)
                .AsQueryable();

            // Only show active properties
            q = q.Where(p => p.IsActive);

            // Filter by location (city or country)
            if (!string.IsNullOrWhiteSpace(query.Location))
            {
                var loc = query.Location.ToLower();
                q = q.Where(p => p.Address.City.ToLower().Contains(loc) || p.Address.Country.ToLower().Contains(loc));
            }

            // Filter by max guests
            if (query.Guests.HasValue)
                q = q.Where(p => p.MaxGuests >= query.Guests.Value);

            // Filter by price
            if (query.MinPrice.HasValue)
                q = q.Where(p => p.PricePerNight >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue)
                q = q.Where(p => p.PricePerNight <= query.MaxPrice.Value);

            // Filter by property type
            if (!string.IsNullOrWhiteSpace(query.PropertyType))
                q = q.Where(p => p.PropertyType == query.PropertyType);

            // Filter by amenities
            if (query.Amenities != null && query.Amenities.Any())
            {
                foreach (var amenity in query.Amenities)
                {
                    var a = amenity;
                    q = q.Where(p => p.Amenities.Any(pa => pa.Name == a));
                }
            }

            // Filter by availability (check-in/check-out)
            if (query.CheckIn.HasValue && query.CheckOut.HasValue)
            {
                var checkIn = query.CheckIn.Value.Date;
                var checkOut = query.CheckOut.Value.Date;

                // Exclude properties with overlapping non-cancelled bookings
                q = q.Where(p => !p.Bookings.Any(b =>
                    b.BookingStatus != "Cancelled" &&
                    b.StartDate < checkOut &&
                    b.EndDate > checkIn));

                // Exclude properties with blocked dates in the range
                q = q.Where(p => !p.BlockedDates.Any(bd =>
                    bd.BlockedDate >= checkIn &&
                    bd.BlockedDate < checkOut));
            }

            // Materialize to compute avg rating
            var properties = q.ToList();

            // Compute average rating
            var dtos = properties.Select(p =>
            {
                var allReviews = p.Bookings.SelectMany(b => b.Reviews).ToList();
                double? avgRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : null;

                return new SearchPropertiesDto
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Name = p.Name,
                    Description = p.Description,
                    PropertyType = p.PropertyType,
                    MaxGuests = p.MaxGuests,
                    IsActive = p.IsActive,
                    IsApproved = p.IsApproved,
                    PricePerNight = p.PricePerNight,
                    AverageRating = avgRating,
                    AddressCity = p.Address?.City,
                    AddressCountry = p.Address?.Country
                };
            }).ToList();

            // Filter by min rating (done after materializing since it requires avg computation)
            if (query.MinRating.HasValue)
                dtos = dtos.Where(d => d.AverageRating.HasValue && d.AverageRating.Value >= query.MinRating.Value).ToList();

            // Sort
            dtos = query.SortBy switch
            {
                "price_asc" => dtos.OrderBy(d => d.PricePerNight).ToList(),
                "price_desc" => dtos.OrderByDescending(d => d.PricePerNight).ToList(),
                "rating" => dtos.OrderByDescending(d => d.AverageRating ?? 0).ToList(),
                "popularity" => dtos.OrderByDescending(d => d.Id).ToList(),
                _ => dtos
            };

            return Task.FromResult(dtos);
        }
    }
}
