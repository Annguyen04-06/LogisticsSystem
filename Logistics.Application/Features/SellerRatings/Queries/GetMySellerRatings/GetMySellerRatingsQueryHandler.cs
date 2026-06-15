using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetMySellerRatings;

public class GetMySellerRatingsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMySellerRatingsQuery, ApiResponse<List<SellerRatingDto>>>
{
    public async Task<ApiResponse<List<SellerRatingDto>>> Handle(
        GetMySellerRatingsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<SellerRatingDto>>.Fail("Only customer can view their seller ratings.");
        }

        var ratings = await SellerRatingDtoBuilder.BuildListAsync(
            context,
            context.SellerRatings.Where(rating => rating.CustomerId == request.CurrentUserId),
            cancellationToken);

        return ApiResponse<List<SellerRatingDto>>.Ok(ratings);
    }
}
