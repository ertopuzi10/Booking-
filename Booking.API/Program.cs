using Booking.Infrastructure.Persistence;
using Booking.Infrastructure;
using Booking.Infrastructure.Services;
using Booking.Application.Features.Users.Register;
using Booking.Application.Services;
using Booking.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// URLs configuration
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// JWT Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-secret-key-must-be-at-least-32-characters-long";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BookingAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BookingAPIClient";
var jwtExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"] ?? "60");

// HttpContext access (for current user from JWT)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Register Infrastructure
builder.Services.RegisterInfrastructure(builder.Configuration);

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// JWT Authentication
var key = Encoding.ASCII.GetBytes(jwtSecret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();