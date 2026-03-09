namespace Booking.Application.Features.Properties.Create
{
    public class CreatePropertyDto
    {
        public int OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;
        public int AddressId { get; set; }
        public int MaxGuests { get; set; }
    }
}