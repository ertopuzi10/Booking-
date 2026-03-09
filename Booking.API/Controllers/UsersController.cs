using Booking.Application.Features.Users.Register;
using Booking.Application.Features.Users.Login;
using Booking.Application.Features.Users.GetAll;
using Booking.Application.Features.Users.GetById;
using Booking.Application.Features.Users.UpdateProfile;
using Booking.Application.Features.Users.ChangePassword;
using Booking.Application.Features.Users.AssignRole;
using Booking.Application.Features.Users.SuspendUser;
using Booking.Application.Features.Users.DeleteUser;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(user);
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
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                PhoneNumber = dto.PhoneNumber
            };

            var userId = await _mediator.Send(command);
            return CreatedAtAction(null, new { id = userId }, new { id = userId });
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

            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request)
        {
            var command = new AssignRoleCommand
            {
                UserId = id,
                RoleName = request.RoleName
            };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/suspend")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuspendUser(int id)
        {
            await _mediator.Send(new SuspendUserCommand { UserId = id });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return NoContent();
        }
    }

    public class AssignRoleRequest
    {
        public string RoleName { get; set; } = null!;
    }
}
