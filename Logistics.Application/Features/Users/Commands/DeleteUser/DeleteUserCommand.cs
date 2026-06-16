using Logistics.Application.Common;
using MediatR;

namespace Logistics.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand(int id, int currentUserId) : IRequest<ApiResponse<bool>>
{
    public int Id { get; } = id;
    public int CurrentUserId { get; } = currentUserId;
}
