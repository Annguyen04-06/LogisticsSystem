using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetSellerRatings;

public class GetSellerRatingsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerRatingsQuery, ApiResponse<List<SellerRatingDto>>>
{
    public async Task<ApiResponse<List<SellerRatingDto>>> Handle(
        GetSellerRatingsQuery request,
        CancellationToken cancellationToken)
    {
        var ratings = await SellerRatingDtoBuilder.BuildListAsync(
            context,
            context.SellerRatings.Where(rating => rating.SellerId == request.SellerId),
            cancellationToken);

        return ApiResponse<List<SellerRatingDto>>.Ok(ratings);
    }
}
