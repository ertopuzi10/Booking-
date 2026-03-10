namespace Booking.Application.Services
{
    // Using JWT via HttpContext instead of accepting user ID from the request for secure, token-based updates.
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? FirstName { get; }
        string? LastName { get; }
        bool IsAuthenticated { get; }
    }
}