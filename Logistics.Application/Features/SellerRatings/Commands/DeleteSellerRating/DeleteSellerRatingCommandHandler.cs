using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.SellerRatings.Commands.DeleteSellerRating;

public class DeleteSellerRatingCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteSellerRatingCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteSellerRatingCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer && request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<bool>.Fail("You are not allowed to delete seller ratings.");
        }

        var sellerRating = await context.SellerRatings
            .FirstOrDefaultAsync(rating => rating.Id == request.Id, cancellationToken);

        if (sellerRating == null)
        {
            return ApiResponse<bool>.Fail("Seller rating does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Customer && sellerRating.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<bool>.Fail("You can only delete your own seller rating.");
        }

        context.SellerRatings.Remove(sellerRating);
        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Seller rating deleted successfully.");
    }
}
