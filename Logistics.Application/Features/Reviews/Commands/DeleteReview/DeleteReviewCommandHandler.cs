using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteReviewCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer && request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<bool>.Fail("You are not allowed to delete reviews.");
        }

        var review = await context.Reviews
            .FirstOrDefaultAsync(review => review.Id == request.Id, cancellationToken);

        if (review == null)
        {
            return ApiResponse<bool>.Fail("Review does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Customer && review.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<bool>.Fail("You can only delete your own review.");
        }

        context.Reviews.Remove(review);
        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Review deleted successfully.");
    }
}
