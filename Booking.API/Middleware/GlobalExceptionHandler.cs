using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Booking.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (status, title, detail) = MapException(ex);

            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            _logger.LogError(ex,
                "Unhandled exception. {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                traceId);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning(
                    "Response already started; cannot write exception response. TraceId: {TraceId}",
                    traceId);
                ExceptionDispatchInfo.Capture(ex).Throw();
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var payload = new ExceptionDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                TraceId = traceId,
                ExceptionType = _env.IsDevelopment() ? ex.GetType().FullName : null,
                StackTrace = _env.IsDevelopment() ? ex.StackTrace : null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
        }

        private static (int status, string title, string? detail) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentException aex => (StatusCodes.Status400BadRequest, "Bad request", aex.Message),
                UnauthorizedAccessException uex => (StatusCodes.Status401Unauthorized, "Unauthorized", uex.Message),
                KeyNotFoundException knf => (StatusCodes.Status404NotFound, "Not found", knf.Message),
                InvalidOperationException ioex => (StatusCodes.Status409Conflict, "Conflict", ioex.Message),
                DbUpdateException dbex => (StatusCodes.Status409Conflict, "Database conflict", dbex.InnerException?.Message ?? dbex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred.")
            };
        }
    }

    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionHandler>();
    }
}