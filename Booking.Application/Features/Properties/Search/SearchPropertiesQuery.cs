using MediatR;
using System;
using System.Collections.Generic;

namespace Booking.Application.Features.Properties.Search
{
    public class SearchPropertiesQuery : IRequest<List<SearchPropertiesDto>>
    {
        public string? Location { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? PropertyType { get; set; }
        public List<string>? Amenities { get; set; }
        public double? MinRating { get; set; }
        public string? SortBy { get; set; }
    }
}
