namespace Booking.Application.Common.Interfaces
{
    public interface IAuthManager
    {
        bool VerifyPassword(string password, string hash);
        string GenerateJwtToken(int userId, string email, string firstName, string lastName);
    }
}
