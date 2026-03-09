namespace Booking.Application.Features.Properties.Update
{
    public class UpdatePropertyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PropertyType { get; set; } = null!;
        public int MaxGuests { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
    }
}