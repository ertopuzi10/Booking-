using System.Security.Claims;
using Booking.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Booking.Infrastructure.Services
{
    /// Reading the current user identity from the JWT claims via IHttpContextAccessor.
    /// Claims are set by AuthManager when generating the token: id, email, firstName, lastName.
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
                return int.TryParse(idClaim, out var id) ? id : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

        public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst("firstName")?.Value;

        public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst("lastName")?.Value;

        public bool IsAuthenticated => UserId.HasValue;
    }
}