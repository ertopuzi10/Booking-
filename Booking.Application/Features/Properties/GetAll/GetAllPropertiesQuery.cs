using MediatR;
using System.Collections.Generic;

namespace Booking.Application.Features.Properties.GetAll
{
    public record GetAllPropertiesQuery : IRequest<List<GetAllPropertiesDto>>;
}