using Booking.Application.Abstractions;
using Booking.Application.Common.Interfaces;
using Booking.Infrastructure.Contracts.AuthService;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BookingDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<BookingDbContext>());

            var jwtSecret = configuration["Jwt:Secret"] ??
                            "your-secret-key-must-be-at-least-32-characters-long";
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "BookingAPI";
            var jwtAudience = configuration["Jwt:Audience"] ?? "BookingAPIClient";
            var jwtExpiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes)
                ? minutes
                : 60;

            services.AddScoped<IAuthManager>(_ =>
                new AuthManager(jwtSecret, jwtIssuer, jwtAudience, jwtExpiryMinutes));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            return services;
        }
    }
}
