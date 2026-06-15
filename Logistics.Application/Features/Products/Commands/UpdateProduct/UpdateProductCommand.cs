using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public int Id { get; }
    public UpdateProductDto Product { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateProductCommand(int id, UpdateProductDto product, int currentUserId, UserRole currentUserRole)
    {
        Id = id;
        Product = product;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
