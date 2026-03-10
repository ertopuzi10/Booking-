using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Services;
using MediatR;

namespace Booking.Application.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, GetBookingByIdDto>
    {
        private readonly IBookingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingByIdHandler(IBookingRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<GetBookingByIdDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in.");

            var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isGuest = booking.GuestId == currentUserId;
            var isHost = booking.Property.OwnerId == currentUserId;

            if (!isGuest && !isHost)
                throw new UnauthorizedAccessException("You do not have access to this booking.");

            return new GetBookingByIdDto
            {
                Id = booking.Id,
                PropertyId = booking.PropertyId,
                PropertyName = booking.Property.Name,
                GuestId = booking.GuestId,
                GuestFullName = $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Nights = (booking.EndDate - booking.StartDate).Days,
                GuestCount = booking.GuestCount,
                PriceForPeriod = booking.PriceForPeriod,
                CleaningFee = booking.CleaningFee,
                ExtraGuestFee = booking.AmenitiesUpCharge,
                ServiceFee = booking.ServiceFee,
                TaxAmount = booking.TaxAmount,
                TotalPrice = booking.TotalPrice,
                BookingStatus = booking.BookingStatus,
                CancellationReason = booking.CancellationReason,
                CreatedAt = booking.CreatedAt,
                ConfirmedOnUtc = booking.ConfirmedOnUtc,
                RejectedOnUtc = booking.RejectedOnUtc,
                CancelledOnUtc = booking.CancelledOnUtc,
                CompletedOnUtc = booking.CompletedOnUtc,
                ExpiredOnUtc = booking.ExpiredOnUtc
            };
        }
    }
}
