namespace Booking.Domain.Entities
{
    public class PropertyPhoto
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public Properties Property { get; set; } = null!;

        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
