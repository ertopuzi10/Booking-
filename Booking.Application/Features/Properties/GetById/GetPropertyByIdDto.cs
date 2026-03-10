using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Properties.GetById
{
    public class PropertyReviewDto
    {
        public int Id { get; init; }
        public string GuestFullName { get; init; } = string.Empty;
        public int Rating { get; init; }
        public string? Comment { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public class GetPropertyByIdDto
    {
        public int Id { get; init; }
        public int OwnerId { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string PropertyType { get; init; } = null!;
        public int AddressId { get; init; }
        public int MaxGuests { get; init; }
        public TimeSpan CheckInTime { get; init; }
        public TimeSpan CheckOutTime { get; init; }
        public decimal PricePerNight { get; init; }
        public string? Rules { get; init; }
        public int MinStayNights { get; init; }
        public int? MaxStayNights { get; init; }
        public bool IsActive { get; init; }
        public bool IsApproved { get; init; }
        public string? AddressCity { get; init; }
        public string? AddressCountry { get; init; }
        public string? AddressStreet { get; init; }
        public List<string> AmenityNames { get; init; } = new List<string>();
        public List<string> PhotoUrls { get; init; } = new List<string>();
        public decimal? AverageRating { get; init; }
        public List<PropertyReviewDto> Reviews { get; init; } = new List<PropertyReviewDto>();

        // Pricing rules — full breakdown so callers can preview costs before booking
        public decimal CleaningFee { get; init; }
        public decimal ExtraGuestFeePerNight { get; init; }
        public int BaseGuestsIncluded { get; init; }
        public decimal ServiceFeePercent { get; init; }
        public decimal TaxPercent { get; init; }
    }
}
