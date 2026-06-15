using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reviews.Queries.GetAllReviews;

public record GetAllReviewsQuery(UserRole CurrentUserRole) : IRequest<ApiResponse<List<ReviewDto>>>;
