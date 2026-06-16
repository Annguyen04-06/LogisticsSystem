using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using MediatR;

namespace Logistics.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery(int id) : IRequest<ApiResponse<UserDto>>
{
    public int Id { get; } = id;
}
