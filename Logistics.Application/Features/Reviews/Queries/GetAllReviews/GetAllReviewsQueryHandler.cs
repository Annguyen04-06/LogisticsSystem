using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reviews.Queries.GetAllReviews;

public class GetAllReviewsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllReviewsQuery, ApiResponse<List<ReviewDto>>>
{
    public async Task<ApiResponse<List<ReviewDto>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<ReviewDto>>.Fail("Only admin can view all reviews.");
        }

        var reviews = await ReviewDtoBuilder.BuildListAsync(context, context.Reviews, cancellationToken);
        return ApiResponse<List<ReviewDto>>.Ok(reviews);
    }
}
