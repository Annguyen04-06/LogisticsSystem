using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using MediatR;

namespace Logistics.Application.Features.Reviews.Queries.GetProductReviews;

public record GetProductReviewsQuery(int ProductId) : IRequest<ApiResponse<List<ReviewDto>>>;
