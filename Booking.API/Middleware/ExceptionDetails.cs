namespace Booking.API.Middleware
{
    public class ExceptionDetails
    {
        public int Status { get; init; }
        public string Title { get; init; } = null!;
        public string? Detail { get; init; }
        public string TraceId { get; init; } = null!;

        public string? ExceptionType { get; init; }
        public string? StackTrace { get; init; }
    }
}
