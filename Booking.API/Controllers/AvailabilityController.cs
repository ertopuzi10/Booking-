using Booking.Application.Features.Availability.BlockDates;
using Booking.Application.Features.Availability.UnblockDates;
using Booking.Application.Features.Availability.SetSeasonalPrice;
using Booking.Application.Features.Availability.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/availability")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvailabilityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{propertyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailability(int propertyId, [FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month < 1 || month > 12)
                return BadRequest("Valid year and month (1-12) are required.");

            var result = await _mediator.Send(new GetAvailabilityQuery(propertyId, year, month));
            return Ok(result);
        }

        [HttpPost("{propertyId}/block")]
        [Authorize]
        public async Task<IActionResult> BlockDates(int propertyId, [FromBody] BlockDatesRequest request)
        {
            var command = new BlockDatesCommand
            {
                PropertyId = propertyId,
                Dates = request.Dates,
                Reason = request.Reason
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{propertyId}/unblock")]
        [Authorize]
        public async Task<IActionResult> UnblockDates(int propertyId, [FromBody] UnblockDatesRequest request)
        {
            var command = new UnblockDatesCommand
            {
                PropertyId = propertyId,
                Dates = request.Dates
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{propertyId}/seasonal-price")]
        [Authorize]
        public async Task<IActionResult> SetSeasonalPrice(int propertyId, [FromBody] SetSeasonalPriceCommand command)
        {
            command.PropertyId = propertyId;
            await _mediator.Send(command);
            return NoContent();
        }
    }

    public class BlockDatesRequest
    {
        public System.Collections.Generic.List<System.DateTime> Dates { get; set; } = new();
        public string? Reason { get; set; }
    }

    public class UnblockDatesRequest
    {
        public System.Collections.Generic.List<System.DateTime> Dates { get; set; } = new();
    }
}
