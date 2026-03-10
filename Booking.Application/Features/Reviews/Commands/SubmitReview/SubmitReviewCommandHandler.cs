using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SubmitReviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to submit a review.");

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // Load the booking
            var booking = await _context.BookingsQuery
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

            // Only the guest who made the booking may review it
            if (booking.GuestId != _currentUserService.UserId.Value)
                throw new UnauthorizedAccessException("You can only review your own bookings.");

            // Booking must be completed before a review can be submitted
            if (booking.BookingStatus != "Completed")
                throw new InvalidOperationException("You can only review a booking after the stay is completed.");

            // Prevent duplicate reviews — one per booking
            var alreadyReviewed = await _context.ReviewsQuery
                .AnyAsync(r => r.BookingId == request.BookingId, cancellationToken);

            if (alreadyReviewed)
                throw new InvalidOperationException("You have already submitted a review for this booking.");

            // Load the property (tracked so EF picks up the AverageRating update)
            var property = await _context.PropertiesQuery
                .FirstOrDefaultAsync(p => p.Id == booking.PropertyId, cancellationToken)
                ?? throw new KeyNotFoundException($"Property {booking.PropertyId} not found.");

            // Create the review
            var review = new Domain.Entities.Reviews
            {
                BookingId = request.BookingId,
                PropertyId = booking.PropertyId,
                GuestId = _currentUserService.UserId.Value,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(review);

         
            // Fetch all existing ratings for this property, then include the new one.
            var existingRatings = await _context.ReviewsQuery
                .Where(r => r.PropertyId == booking.PropertyId)
                .Select(r => r.Rating)
                .ToListAsync(cancellationToken);

            existingRatings.Add(request.Rating);
            property.AverageRating = Math.Round((decimal)existingRatings.Average(), 2);

            // Both the new review and the updated AverageRating are saved in one transaction
            await _context.SaveChangesAsync(cancellationToken);

            return review.Id;
        }
    }
}
