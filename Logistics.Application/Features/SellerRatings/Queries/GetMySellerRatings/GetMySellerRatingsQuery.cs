using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetMySellerRatings;

public record GetMySellerRatingsQuery(
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<List<SellerRatingDto>>>;
