using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Commands.CreateSellerRating;

public record CreateSellerRatingCommand(
    CreateSellerRatingDto SellerRating,
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<SellerRatingDto>>;
