using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetAllSellerRatings;

public class GetAllSellerRatingsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllSellerRatingsQuery, ApiResponse<List<SellerRatingDto>>>
{
    public async Task<ApiResponse<List<SellerRatingDto>>> Handle(
        GetAllSellerRatingsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<SellerRatingDto>>.Fail("Only admin can view all seller ratings.");
        }

        var ratings = await SellerRatingDtoBuilder.BuildListAsync(context, context.SellerRatings, cancellationToken);
        return ApiResponse<List<SellerRatingDto>>.Ok(ratings);
    }
}
