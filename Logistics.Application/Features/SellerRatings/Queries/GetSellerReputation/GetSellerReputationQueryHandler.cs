using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.SellerRatings.Queries.GetSellerReputation;

public class GetSellerReputationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerReputationQuery, ApiResponse<SellerReputationDto>>
{
    public async Task<ApiResponse<SellerReputationDto>> Handle(
        GetSellerReputationQuery request,
        CancellationToken cancellationToken)
    {
        var seller = await context.Users
            .FirstOrDefaultAsync(
                user => user.Id == request.SellerId && user.Role == UserRole.Seller,
                cancellationToken);

        if (seller == null)
        {
            return ApiResponse<SellerReputationDto>.Fail("Seller does not exist.");
        }

        var totalLikes = await context.SellerRatings
            .CountAsync(rating => rating.SellerId == request.SellerId && rating.IsLike, cancellationToken);
        var totalDislikes = await context.SellerRatings
            .CountAsync(rating => rating.SellerId == request.SellerId && !rating.IsLike, cancellationToken);

        var totalRatings = totalLikes + totalDislikes;
        var trustPercent = totalRatings == 0 ? 0 : (decimal)totalLikes / totalRatings * 100;

        var reputation = new SellerReputationDto
        {
            SellerId = seller.Id,
            SellerName = seller.FullName,
            TotalLikes = totalLikes,
            TotalDislikes = totalDislikes,
            ReputationScore = totalLikes - totalDislikes,
            TrustPercent = trustPercent
        };

        return ApiResponse<SellerReputationDto>.Ok(reputation);
    }
}
