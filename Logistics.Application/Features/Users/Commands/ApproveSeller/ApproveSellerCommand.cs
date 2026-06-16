using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using MediatR;

namespace Logistics.Application.Features.Users.Commands.ApproveSeller;

public class ApproveSellerCommand(int id) : IRequest<ApiResponse<UserDto>>
{
    public int Id { get; } = id;
}
