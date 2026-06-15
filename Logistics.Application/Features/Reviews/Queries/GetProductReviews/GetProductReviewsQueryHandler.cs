using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Interfaces;
using MediatR;

namespace Logistics.Application.Features.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductReviewsQuery, ApiResponse<List<ReviewDto>>>
{
    public async Task<ApiResponse<List<ReviewDto>>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await ReviewDtoBuilder.BuildListAsync(
            context,
            context.Reviews.Where(review => review.ProductId == request.ProductId),
            cancellationToken);

        return ApiResponse<List<ReviewDto>>.Ok(reviews);
    }
}
