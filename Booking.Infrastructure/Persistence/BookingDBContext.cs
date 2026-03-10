using Booking.Domain.Entities;
using Booking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Persistence
{
    public class BookingDbContext : DbContext, IApplicationDbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Properties> Properties { get; set; }
        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<OwnerProfiles> OwnerProfiles { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<PropertyPhoto> PropertyPhotos { get; set; }
        public DbSet<PropertyAvailability> PropertyAvailabilities { get; set; }
        public DbSet<PropertySeasonalPrice> PropertySeasonalPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users Configuration
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.IsSuspended).HasDefaultValue(false);
            });

            // Addresses Configuration
            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Street).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
            });

            // Properties Configuration
            modelBuilder.Entity<Properties>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.PropertyType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MinStayNights).HasDefaultValue(1);
                entity.Property(e => e.CleaningFee).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                entity.Property(e => e.ExtraGuestFeePerNight).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                entity.Property(e => e.BaseGuestsIncluded).HasDefaultValue(2);
                entity.Property(e => e.ServiceFeePercent).HasColumnType("decimal(5,4)").HasDefaultValue(0.10m);
                entity.Property(e => e.TaxPercent).HasColumnType("decimal(5,4)").HasDefaultValue(0.085m);
                entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)").IsRequired(false);

                // Relationship: Property -> Owner (User)
                entity.HasOne(e => e.Owner)
                    .WithMany(u => u.Properties)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Property -> Address
                entity.HasOne(e => e.Address)
                    .WithMany(a => a.Properties)
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Bookings Configuration
            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookingStatus).HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CleaningFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AmenitiesUpCharge).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PriceForPeriod).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ServiceFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");

                // Relationship: Booking -> Property
                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Booking -> Guest (User)
                entity.HasOne(e => e.Guest)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Reviews Configuration
            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment);

                // One review per booking (prevents duplicates at the DB level)
                entity.HasIndex(e => e.BookingId).IsUnique();

                // Relationship: Review -> Booking
                entity.HasOne(e => e.Bookings)
                    .WithMany(b => b.Reviews)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Review -> Property (direct link)
                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relationship: Review -> Guest (User)
                entity.HasOne(e => e.Guest)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Roles Configuration
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasData(
                    new Roles { Id = 1, Name = "Guest", Description = "Default user role", IsDefault = true },
                    new Roles { Id = 2, Name = "Host", Description = "Property owner role", IsDefault = false },
                    new Roles { Id = 3, Name = "Admin", Description = "Administrator role", IsDefault = false }
                );
            });

            // UserRoles Configuration (Many-to-Many between Users and Roles)
            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OwnerProfiles Configuration
            modelBuilder.Entity<OwnerProfiles>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.IdentityCardNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BusinessName).HasMaxLength(255);
                entity.Property(e => e.CreditCard).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithOne(u => u.OwnerProfile)
                    .HasForeignKey<OwnerProfiles>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PropertyAmenity Configuration
            modelBuilder.Entity<PropertyAmenity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Amenities)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PropertyPhoto Configuration
            modelBuilder.Entity<PropertyPhoto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).IsRequired();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PropertyAvailability Configuration
            modelBuilder.Entity<PropertyAvailability>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BlockedDate).IsRequired();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.BlockedDates)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PropertySeasonalPrice Configuration
            modelBuilder.Entity<PropertySeasonalPrice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.SeasonalPrices)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // IApplicationDbContext explicit implementations
        IQueryable<Users> IApplicationDbContext.UsersQuery => Users;
        IQueryable<Roles> IApplicationDbContext.RolesQuery => Roles;
        IQueryable<UserRoles> IApplicationDbContext.UserRolesQuery => UserRoles;
        IQueryable<Properties> IApplicationDbContext.PropertiesQuery => Properties;
        IQueryable<PropertyAmenity> IApplicationDbContext.PropertyAmenitiesQuery => PropertyAmenities;
        IQueryable<PropertyPhoto> IApplicationDbContext.PropertyPhotosQuery => PropertyPhotos;
        IQueryable<PropertyAvailability> IApplicationDbContext.PropertyAvailabilitiesQuery => PropertyAvailabilities;
        IQueryable<PropertySeasonalPrice> IApplicationDbContext.SeasonalPricesQuery => PropertySeasonalPrices;
        IQueryable<Bookings> IApplicationDbContext.BookingsQuery => Bookings;
        IQueryable<Reviews> IApplicationDbContext.ReviewsQuery => Reviews;

        void IApplicationDbContext.Add<TEntity>(TEntity entity) => Set<TEntity>().Add(entity!);
        void IApplicationDbContext.Remove<TEntity>(TEntity entity) => Set<TEntity>().Remove(entity!);
        Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken) => base.SaveChangesAsync(cancellationToken);
    }
}
