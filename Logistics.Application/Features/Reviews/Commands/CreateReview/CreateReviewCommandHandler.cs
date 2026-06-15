using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateReviewCommand, ApiResponse<ReviewDto>>
{
    public async Task<ApiResponse<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<ReviewDto>.Fail("Only customer can create review.");
        }

        if (request.Review.Rating < 1 || request.Review.Rating > 5)
        {
            return ApiResponse<ReviewDto>.Fail("Rating must be from 1 to 5.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.Review.OrderId, cancellationToken);

        if (order == null)
        {
            return ApiResponse<ReviewDto>.Fail("Order does not exist.");
        }

        if (order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<ReviewDto>.Fail("You can only review your own order.");
        }

        if (order.Status != OrderStatus.Delivered)
        {
            return ApiResponse<ReviewDto>.Fail("Only delivered orders can be reviewed.");
        }

        var productInOrder = await context.OrderDetails
            .AnyAsync(
                detail => detail.OrderId == request.Review.OrderId && detail.ProductId == request.Review.ProductId,
                cancellationToken);

        if (!productInOrder)
        {
            return ApiResponse<ReviewDto>.Fail("Product is not in this order.");
        }

        var alreadyReviewed = await context.Reviews
            .AnyAsync(
                review => review.OrderId == request.Review.OrderId
                    && review.ProductId == request.Review.ProductId
                    && review.CustomerId == request.CurrentUserId,
                cancellationToken);

        if (alreadyReviewed)
        {
            return ApiResponse<ReviewDto>.Fail("You already reviewed this product for this order.");
        }

        var review = new Review
        {
            OrderId = request.Review.OrderId,
            ProductId = request.Review.ProductId,
            CustomerId = request.CurrentUserId,
            Rating = request.Review.Rating,
            Comment = request.Review.Comment.Trim()
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        var reviewDto = await ReviewDtoBuilder.BuildOneAsync(
            context,
            context.Reviews.Where(item => item.Id == review.Id),
            cancellationToken);

        return ApiResponse<ReviewDto>.Ok(reviewDto!, "Review created successfully.");
    }
}
