using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reviews.Queries.GetMyReviews;

public class GetMyReviewsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyReviewsQuery, ApiResponse<List<ReviewDto>>>
{
    public async Task<ApiResponse<List<ReviewDto>>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<ReviewDto>>.Fail("Only customer can view their reviews.");
        }

        var reviews = await ReviewDtoBuilder.BuildListAsync(
            context,
            context.Reviews.Where(review => review.CustomerId == request.CurrentUserId),
            cancellationToken);

        return ApiResponse<List<ReviewDto>>.Ok(reviews);
    }
}
