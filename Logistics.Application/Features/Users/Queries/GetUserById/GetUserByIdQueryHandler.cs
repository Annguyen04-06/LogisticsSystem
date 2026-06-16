using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.Fail("Không tìm thấy người dùng.");
        }

        return ApiResponse<UserDto>.Ok(UserMapping.ToDto(user));
    }
}
