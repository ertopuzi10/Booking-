using Booking.Application.Features.Bookings.Commands.CancelBooking;
using Booking.Application.Features.Bookings.Commands.ConfirmBooking;
using Booking.Application.Features.Bookings.Commands.CreateBooking;
using Booking.Application.Features.Bookings.Commands.RejectBooking;
using Booking.Application.Features.Bookings.Queries.GetBookingById;
using Booking.Application.Features.Bookings.Queries.GetBookingHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

 
        // Create a new booking where GuestId is read from the JWT token.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }


        // Get details of a specific booking. Accessible by the guest or property owner.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = id });
            return Ok(result);
        }

        // Get booking history for the current user (as guest or host).
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string? status)
        {
            var result = await _mediator.Send(new GetBookingHistoryQuery { Status = status });
            return Ok(result);
        }

        // Confirm a pending booking
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _mediator.Send(new ConfirmBookingCommand { BookingId = id });
            return NoContent();
        }

        // Reject a pending booking
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectBookingRequest request)
        {
            await _mediator.Send(new RejectBookingCommand { BookingId = id, Reason = request.Reason });
            return NoContent();
        }

        // Cancel a booking. Accessible by the guest or property owner.
        // Returns the cancellation policy result including refund amount.
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelBookingRequest request)
        {
            var result = await _mediator.Send(new CancelBookingCommand { BookingId = id, Reason = request.Reason });
            return Ok(result);
        }
    }

    public class RejectBookingRequest
    {
        public string? Reason { get; set; }
    }

    public class CancelBookingRequest
    {
        public string? Reason { get; set; }
    }
}
