using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Services;
using MediatR;

namespace Booking.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, CancelBookingResult>
    {
        private readonly IBookingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CancelBookingCommandHandler(IBookingRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<CancelBookingResult> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in.");

            var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

            var currentUserId = _currentUserService.UserId.Value;
            var isGuest = booking.GuestId == currentUserId;
            var isHost = booking.Property.OwnerId == currentUserId;

            if (!isGuest && !isHost)
                throw new UnauthorizedAccessException("Only the guest or property owner can cancel this booking.");

            if (booking.BookingStatus != "Pending" && booking.BookingStatus != "Confirmed")
                throw new InvalidOperationException($"Cannot cancel a booking with status '{booking.BookingStatus}'.");

            var daysUntilCheckIn = (booking.StartDate.Date - DateTime.UtcNow.Date).Days;
            var result = ApplyCancellationPolicy(booking.TotalPrice, daysUntilCheckIn);

            booking.BookingStatus = "Cancelled";
            booking.CancellationReason = request.Reason;
            booking.CancelledOnUtc = DateTime.UtcNow;
            booking.LastModifiedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync(cancellationToken);

            return result;
        }

        private static CancelBookingResult ApplyCancellationPolicy(decimal totalPrice, int daysUntilCheckIn)
        {
            if (daysUntilCheckIn > 7)
            {
                return new CancelBookingResult
                {
                    PolicyDescription = "Full refund — cancelled more than 7 days before check-in.",
                    RefundAmount = totalPrice,
                    PenaltyAmount = 0
                };
            }

            if (daysUntilCheckIn >= 3)
            {
                var refund = Math.Round(totalPrice * 0.5m, 2);
                return new CancelBookingResult
                {
                    PolicyDescription = "50% refund — cancelled 3–7 days before check-in.",
                    RefundAmount = refund,
                    PenaltyAmount = totalPrice - refund
                };
            }

            return new CancelBookingResult
            {
                PolicyDescription = "No refund — cancelled less than 3 days before check-in.",
                RefundAmount = 0,
                PenaltyAmount = totalPrice
            };
        }
    }
}
