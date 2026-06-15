using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; }
    public UserRole CurrentUserRole { get; }

    public DeleteCategoryCommand(int id, UserRole currentUserRole)
    {
        Id = id;
        CurrentUserRole = currentUserRole;
    }
}
