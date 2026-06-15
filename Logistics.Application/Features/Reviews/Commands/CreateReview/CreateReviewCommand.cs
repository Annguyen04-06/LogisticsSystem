using Logistics.Application.Common;
using Logistics.Application.DTOs.Reviews;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    CreateReviewDto Review,
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<ReviewDto>>;
