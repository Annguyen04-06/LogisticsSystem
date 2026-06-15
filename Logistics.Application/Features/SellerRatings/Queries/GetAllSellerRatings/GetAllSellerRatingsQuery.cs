using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetAllSellerRatings;

public record GetAllSellerRatingsQuery(UserRole CurrentUserRole) : IRequest<ApiResponse<List<SellerRatingDto>>>;
