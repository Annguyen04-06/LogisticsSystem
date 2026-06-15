using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public DeleteProductCommand(int id, int currentUserId, UserRole currentUserRole)
    {
        Id = id;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
