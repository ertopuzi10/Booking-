using Microsoft.Extensions.DependencyInjection;
using Booking.Application.Common.Interfaces;
using Booking.Infrastructure.Contracts.AuthService;

namespace Booking.Infrastructure
{
    public static class InfrastructureService
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            string jwtSecret,
            string jwtIssuer,
            string jwtAudience,
            int jwtExpiryMinutes = 60)
        {
            services.AddScoped<IAuthManager>(provider =>
                new AuthManager(jwtSecret, jwtIssuer, jwtAudience, jwtExpiryMinutes));

            return services;
        }
    }
}