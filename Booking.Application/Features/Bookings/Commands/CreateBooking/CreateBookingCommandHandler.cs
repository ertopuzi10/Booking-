using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
    {
        private readonly IBookingRepository _repository;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateBookingCommandHandler(
            IBookingRepository repository,
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                throw new UnauthorizedAccessException("You must be logged in to create a booking.");

            if (request.StartDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("StartDate cannot be in the past.");

            if (request.StartDate >= request.EndDate)
                throw new ArgumentException("StartDate must be before EndDate.");

            if (request.GuestCount <= 0)
                throw new ArgumentException("GuestCount must be at least 1.");

            var property = await _context.PropertiesQuery
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.IsActive && p.IsApproved, cancellationToken)
                ?? throw new KeyNotFoundException($"Property {request.PropertyId} not found or not available.");

            if (request.GuestCount > property.MaxGuests)
                throw new ArgumentException($"GuestCount exceeds the property limit of {property.MaxGuests} guests.");

            var nights = (request.EndDate.Date - request.StartDate.Date).Days;

            if (nights < property.MinStayNights)
                throw new ArgumentException($"Minimum stay is {property.MinStayNights} night(s).");

            if (property.MaxStayNights.HasValue && nights > property.MaxStayNights.Value)
                throw new ArgumentException($"Maximum stay is {property.MaxStayNights.Value} night(s).");

            // Check host-blocked dates
            var hasBlockedDates = await _context.PropertyAvailabilitiesQuery
                .AnyAsync(a => a.PropertyId == request.PropertyId
                            && a.BlockedDate >= request.StartDate.Date
                            && a.BlockedDate < request.EndDate.Date,
                          cancellationToken);

            if (hasBlockedDates)
                throw new InvalidOperationException("The property is not available for one or more of the selected dates.");

            // Check conflicting bookings (Pending or Confirmed)
            var hasConflict = await _context.BookingsQuery
                .AnyAsync(b => b.PropertyId == request.PropertyId
                            && (b.BookingStatus == "Pending" || b.BookingStatus == "Confirmed")
                            && b.StartDate < request.EndDate
                            && b.EndDate > request.StartDate,
                          cancellationToken);

            if (hasConflict)
                throw new InvalidOperationException("The property is already booked for the selected dates.");

            // Calculate base price per night, respecting seasonal overrides
            var seasonalPrices = await _context.SeasonalPricesQuery
                .Where(sp => sp.PropertyId == request.PropertyId
                          && sp.StartDate <= request.EndDate
                          && sp.EndDate >= request.StartDate)
                .ToListAsync(cancellationToken);

            decimal priceForPeriod = 0;
            for (var date = request.StartDate.Date; date < request.EndDate.Date; date = date.AddDays(1))
            {
                var seasonal = seasonalPrices
                    .Where(sp => sp.StartDate <= date && sp.EndDate > date)
                    .OrderByDescending(sp => sp.StartDate)
                    .FirstOrDefault();
                priceForPeriod += seasonal?.PricePerNight ?? property.PricePerNight;
            }

            // Apply property-level pricing rules
            var extraGuests = Math.Max(0, request.GuestCount - property.BaseGuestsIncluded);
            var extraGuestFee = extraGuests * property.ExtraGuestFeePerNight * nights;
            var serviceFee = priceForPeriod * property.ServiceFeePercent;
            var taxAmount = (priceForPeriod + extraGuestFee + property.CleaningFee + serviceFee) * property.TaxPercent;
            var totalPrice = priceForPeriod + extraGuestFee + property.CleaningFee + serviceFee + taxAmount;

            var booking = new Domain.Entities.Bookings
            {
                PropertyId = request.PropertyId,
                GuestId = _currentUserService.UserId.Value,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                GuestCount = request.GuestCount,
                PriceForPeriod = Math.Round(priceForPeriod, 2),
                CleaningFee = Math.Round(property.CleaningFee, 2),
                AmenitiesUpCharge = Math.Round(extraGuestFee, 2),
                ServiceFee = Math.Round(serviceFee, 2),
                TaxAmount = Math.Round(taxAmount, 2),
                TotalPrice = Math.Round(totalPrice, 2),
                BookingStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(booking, cancellationToken);

            return booking.Id;
        }
    }
}
