using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Commands.DeleteSellerRating;

public record DeleteSellerRatingCommand(
    int Id,
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<bool>>;
