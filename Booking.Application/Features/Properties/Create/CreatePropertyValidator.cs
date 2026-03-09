namespace Booking.Application.Features.Properties.Create
{
    // Simple manual validator to keep Application layer free of external packages.
    public class CreatePropertyValidator
    {
        public void Validate(CreatePropertyCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length > 255)
                throw new System.ArgumentException("Name is required and must be at most 255 characters long", nameof(command.Name));

            if (string.IsNullOrWhiteSpace(command.Description))
                throw new System.ArgumentException("Description is required", nameof(command.Description));

            if (string.IsNullOrWhiteSpace(command.PropertyType) || command.PropertyType.Length > 50)
                throw new System.ArgumentException("PropertyType is required and must be at most 50 characters long", nameof(command.PropertyType));

            if (command.AddressId <= 0)
                throw new System.ArgumentException("AddressId must be greater than zero", nameof(command.AddressId));

            if (command.MaxGuests <= 0)
                throw new System.ArgumentException("MaxGuests must be greater than zero", nameof(command.MaxGuests));

            if (command.PricePerNight <= 0)
                throw new System.ArgumentException("PricePerNight must be greater than zero", nameof(command.PricePerNight));
        }
    }
}
