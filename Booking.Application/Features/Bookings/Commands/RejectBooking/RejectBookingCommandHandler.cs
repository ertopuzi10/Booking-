using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Services;
using MediatR;

namespace Booking.Application.Features.Bookings.Commands.RejectBooking
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, Unit>
    {
        private readonly IBookingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public RejectBookingCommandHandler(IBookingRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in.");

            var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

            if (booking.Property.OwnerId != _currentUserService.UserId.Value)
                throw new UnauthorizedAccessException("Only the property owner can reject bookings.");

            if (booking.BookingStatus != "Pending")
                throw new InvalidOperationException($"Cannot reject a booking with status '{booking.BookingStatus}'. Only Pending bookings can be rejected.");

            booking.BookingStatus = "Rejected";
            booking.CancellationReason = request.Reason;
            booking.RejectedOnUtc = DateTime.UtcNow;
            booking.LastModifiedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
