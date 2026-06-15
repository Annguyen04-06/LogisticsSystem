using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Products.Queries.GetSellerProducts;

public class GetSellerProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }
    public int? SellerId { get; }

    public GetSellerProductsQuery(int currentUserId, UserRole currentUserRole, int? sellerId)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
        SellerId = sellerId;
    }
}
