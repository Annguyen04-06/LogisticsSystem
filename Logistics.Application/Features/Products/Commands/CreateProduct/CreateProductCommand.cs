using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public CreateProductDto Product { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public CreateProductCommand(CreateProductDto product, int currentUserId, UserRole currentUserRole)
    {
        Product = product;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
