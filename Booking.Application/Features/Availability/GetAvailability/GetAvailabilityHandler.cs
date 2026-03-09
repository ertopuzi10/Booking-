using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Availability.GetAvailability
{
    public class GetAvailabilityHandler : IRequestHandler<GetAvailabilityQuery, GetAvailabilityDto>
    {
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IApplicationDbContext _context;

        public GetAvailabilityHandler(IPropertyAvailabilityRepository availabilityRepository, IApplicationDbContext context)
        {
            _availabilityRepository = availabilityRepository;
            _context = context;
        }

        public async Task<GetAvailabilityDto> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var propertyExists = _context.PropertiesQuery.Any(p => p.Id == request.PropertyId);
            if (!propertyExists)
                throw new KeyNotFoundException($"Property with id {request.PropertyId} not found.");

            var blockedAvailabilities = await _availabilityRepository.GetByPropertyAndMonthAsync(
                request.PropertyId, request.Year, request.Month, cancellationToken);

            var blockedDates = blockedAvailabilities.Select(a => a.BlockedDate.Date).ToList();

            var monthStart = new DateTime(request.Year, request.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var bookedPeriods = _context.BookingsQuery
                .Where(b => b.PropertyId == request.PropertyId
                    && b.BookingStatus != "Cancelled"
                    && b.StartDate < monthEnd
                    && b.EndDate > monthStart)
                .Select(b => new BookedPeriodDto
                {
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    BookingStatus = b.BookingStatus
                })
                .ToList();

            return new GetAvailabilityDto
            {
                PropertyId = request.PropertyId,
                Year = request.Year,
                Month = request.Month,
                BlockedDates = blockedDates,
                BookedPeriods = bookedPeriods
            };
        }
    }
}
