using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetSellerRatings;

public record GetSellerRatingsQuery(int SellerId) : IRequest<ApiResponse<List<SellerRatingDto>>>;
