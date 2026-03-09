using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.Infrastructure
{
    public class DesignTimeBookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BookingDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True");
            return new BookingDbContext(optionsBuilder.Options);
        }
    }
}