using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserDto>>>
{
    public async Task<ApiResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await context.Users
            .OrderByDescending(user => user.CreatedAt)
            .Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                IsApproved = user.IsApproved,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<UserDto>>.Ok(users);
    }
}
