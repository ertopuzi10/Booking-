using System.Collections.Generic;
using MediatR;

namespace Booking.Application.Features.Reviews.Queries.GetReviewsByProperty
{
    public class GetReviewsByPropertyQuery : IRequest<List<GetReviewsByPropertyDto>>
    {
        public int PropertyId { get; set; }
    }
}
