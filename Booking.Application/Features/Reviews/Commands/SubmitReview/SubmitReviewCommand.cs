using MediatR;

namespace Booking.Application.Features.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommand : IRequest<int>
    {
        public int BookingId { get; set; }
        public int Rating { get; set; } 
        public string? Comment { get; set; }
    }
}
