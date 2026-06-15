using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Queries.GetMyCart;

public class GetMyCartQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyCartQuery, ApiResponse<CartDto>>
{
    public async Task<ApiResponse<CartDto>> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<CartDto>.Fail("Only customer can use cart.");
        }

        var cart = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
        return ApiResponse<CartDto>.Ok(cart);
    }
}
