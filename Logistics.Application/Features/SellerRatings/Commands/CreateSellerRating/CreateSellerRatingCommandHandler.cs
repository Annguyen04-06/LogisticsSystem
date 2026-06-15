using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.SellerRatings.Commands.CreateSellerRating;

public class CreateSellerRatingCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSellerRatingCommand, ApiResponse<SellerRatingDto>>
{
    public async Task<ApiResponse<SellerRatingDto>> Handle(
        CreateSellerRatingCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<SellerRatingDto>.Fail("Only customer can rate seller.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.SellerRating.OrderId, cancellationToken);

        if (order == null)
        {
            return ApiResponse<SellerRatingDto>.Fail("Order does not exist.");
        }

        if (order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<SellerRatingDto>.Fail("You can only rate seller from your own order.");
        }

        if (order.Status != OrderStatus.Delivered)
        {
            return ApiResponse<SellerRatingDto>.Fail("Only delivered orders can be rated.");
        }

        if (order.SellerId != request.SellerRating.SellerId)
        {
            return ApiResponse<SellerRatingDto>.Fail("Seller does not match this order.");
        }

        var alreadyRated = await context.SellerRatings
            .AnyAsync(
                rating => rating.OrderId == request.SellerRating.OrderId
                    && rating.SellerId == request.SellerRating.SellerId
                    && rating.CustomerId == request.CurrentUserId,
                cancellationToken);

        if (alreadyRated)
        {
            return ApiResponse<SellerRatingDto>.Fail("You already rated this seller for this order.");
        }

        var sellerRating = new SellerRating
        {
            OrderId = request.SellerRating.OrderId,
            SellerId = request.SellerRating.SellerId,
            CustomerId = request.CurrentUserId,
            IsLike = request.SellerRating.IsLike,
            Comment = request.SellerRating.Comment.Trim()
        };

        context.SellerRatings.Add(sellerRating);
        await context.SaveChangesAsync(cancellationToken);

        var sellerRatingDto = await SellerRatingDtoBuilder.BuildOneAsync(
            context,
            context.SellerRatings.Where(item => item.Id == sellerRating.Id),
            cancellationToken);

        return ApiResponse<SellerRatingDto>.Ok(sellerRatingDto!, "Seller rating created successfully.");
    }
}
