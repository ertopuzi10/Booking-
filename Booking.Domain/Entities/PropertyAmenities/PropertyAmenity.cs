namespace Booking.Domain.Entities
{
    public class PropertyAmenity
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public Properties Property { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
