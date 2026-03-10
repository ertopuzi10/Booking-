using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Reviews.Queries.GetReviewsByProperty
{
    public class GetReviewsByPropertyHandler : IRequestHandler<GetReviewsByPropertyQuery, List<GetReviewsByPropertyDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetReviewsByPropertyHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetReviewsByPropertyDto>> Handle(GetReviewsByPropertyQuery request, CancellationToken cancellationToken)
        {
            var propertyExists = await _context.PropertiesQuery
                .AnyAsync(p => p.Id == request.PropertyId, cancellationToken);

            if (!propertyExists)
                throw new KeyNotFoundException($"Property {request.PropertyId} not found.");

            var reviews = await _context.ReviewsQuery
                .Where(r => r.PropertyId == request.PropertyId)
                .Include(r => r.Guest)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new GetReviewsByPropertyDto
            {
                Id = r.Id,
                BookingId = r.BookingId,
                GuestFullName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}
