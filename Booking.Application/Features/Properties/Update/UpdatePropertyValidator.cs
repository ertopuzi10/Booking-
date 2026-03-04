namespace Booking.Application.Features.Properties.Update
{
    // Simple manual validator to keep Application layer free of external packages.
    public class UpdatePropertyValidator
    {
        public void Validate(UpdatePropertyCommand command)
        {
            if (command.Id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(command.Id));

            if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length > 255)
                throw new ArgumentException("Name is required and must be at most 255 characters long", nameof(command.Name));

            if (string.IsNullOrWhiteSpace(command.Description))
                throw new ArgumentException("Description is required", nameof(command.Description));

            if (string.IsNullOrWhiteSpace(command.PropertyType) || command.PropertyType.Length > 50)
                throw new ArgumentException("PropertyType is required and must be at most 50 characters long", nameof(command.PropertyType));

            if (command.MaxGuests <= 0)
                throw new ArgumentException("MaxGuests must be greater than zero", nameof(command.MaxGuests));
        }
    }
}

