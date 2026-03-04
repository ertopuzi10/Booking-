namespace Booking.Application.Features.Properties.GetById
{
    public class GetPropertyByIdDto
    {
        public int Id { get; init; }
        public int OwnerId { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string PropertyType { get; init; } = null!;
        public int AddressId { get; init; }
        public int MaxGuests { get; init; }
        public bool IsActive { get; init; }
        public bool IsApproved { get; init; }
    }
}

