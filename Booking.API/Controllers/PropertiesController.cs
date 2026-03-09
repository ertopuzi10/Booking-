using Booking.Application.Features.Properties.GetAll;
using Booking.Application.Features.Properties.GetById;
using Booking.Application.Features.Properties.Create;
using Booking.Application.Features.Properties.Update;
using Booking.Application.Features.Properties.Delete;
using Booking.Application.Features.Properties.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPropertiesQuery());
            return Ok(result);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] SearchPropertiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetPropertyByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Route id and body id must match.");
            }

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeletePropertyCommand(id));
            return NoContent();
        }
    }
}
