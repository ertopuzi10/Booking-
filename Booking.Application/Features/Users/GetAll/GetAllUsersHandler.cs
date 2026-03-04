using Booking.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Application.Features.Users.GetAll
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<GetAllUsersDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUsersHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _context.UsersQuery
                .Select(u => new GetAllUsersDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsActive = u.IsActive
                })
                .ToList();

            return Task.FromResult(users);
        }
    }
}

