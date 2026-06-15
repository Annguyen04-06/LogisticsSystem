using Logistics.Application.Common;
using Logistics.Application.DTOs.SellerRatings;
using MediatR;

namespace Logistics.Application.Features.SellerRatings.Queries.GetSellerReputation;

public record GetSellerReputationQuery(int SellerId) : IRequest<ApiResponse<SellerReputationDto>>;
