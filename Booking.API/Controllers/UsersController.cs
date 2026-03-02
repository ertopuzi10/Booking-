using Booking.Application.Features.Users.Register;
using Booking.Application.Features.Users.Login;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            if (dto == null)
                return BadRequest();

            var command = new RegisterUserCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                IsAdmin = dto.IsAdmin
            };

            try
            {
                var userId = await _mediator.Send(command);
                return CreatedAtAction(null, new { id = userId }, new { id = userId });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if (dto == null)
                return BadRequest("Email and password are required");

            var command = new LoginUserCommand
            {
                Email = dto.Email,
                Password = dto.Password
            };

            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
