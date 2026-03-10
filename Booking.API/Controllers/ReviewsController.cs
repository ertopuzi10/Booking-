using Booking.Application.Features.Reviews.Commands.SubmitReview;
using Booking.Application.Features.Reviews.Queries.GetReviewsByProperty;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Submit a review for a completed booking, which should be the booking guest
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] SubmitReviewCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByProperty), new { propertyId = 0 }, new { id });
        }

        // Get all reviews for a property, ordered by most recent first.
        [HttpGet("property/{propertyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            var result = await _mediator.Send(new GetReviewsByPropertyQuery { PropertyId = propertyId });
            return Ok(result);
        }
    }
}
