using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Users.Queries.GetActiveShippers;

public class GetActiveShippersQuery : IRequest<ApiResponse<List<AvailableShipperDto>>>
{
    public UserRole CurrentUserRole { get; }

    public GetActiveShippersQuery(UserRole currentUserRole)
    {
        CurrentUserRole = currentUserRole;
    }
}
