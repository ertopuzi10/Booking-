using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Services;
using MediatR;

namespace Booking.Application.Features.Bookings.Queries.GetBookingHistory
{
    public class GetBookingHistoryHandler : IRequestHandler<GetBookingHistoryQuery, List<GetBookingHistoryDto>>
    {
        private readonly IBookingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingHistoryHandler(IBookingRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<List<GetBookingHistoryDto>> Handle(GetBookingHistoryQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in.");

            var bookings = await _repository.GetByUserAsync(
                _currentUserService.UserId.Value,
                request.Status,
                cancellationToken);

            var today = DateTime.UtcNow.Date;

            return bookings.Select(b => new GetBookingHistoryDto
            {
                Id = b.Id,
                PropertyId = b.PropertyId,
                PropertyName = b.Property.Name,
                PropertyType = b.Property.PropertyType,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Nights = (b.EndDate - b.StartDate).Days,
                GuestCount = b.GuestCount,
                TotalPrice = b.TotalPrice,
                BookingStatus = b.BookingStatus,
                CreatedAt = b.CreatedAt,
                IsUpcoming = b.StartDate.Date >= today
            }).ToList();
        }
    }
}
