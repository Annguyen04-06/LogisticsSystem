using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Queries.GetActiveShippers;

public class GetActiveShippersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetActiveShippersQuery, ApiResponse<List<AvailableShipperDto>>>
{
    public async Task<ApiResponse<List<AvailableShipperDto>>> Handle(
        GetActiveShippersQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<List<AvailableShipperDto>>.Fail("Bạn không có quyền xem danh sách người giao hàng.");
        }

        var shippers = await context.Users
            .Where(user =>
                user.Role == UserRole.Shipper
                && user.IsActive
                && user.IsApproved)
            .OrderBy(user => user.FullName)
            .Select(user => new AvailableShipperDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<AvailableShipperDto>>.Ok(shippers);
    }
}
