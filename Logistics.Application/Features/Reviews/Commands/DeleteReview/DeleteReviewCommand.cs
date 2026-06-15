using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(
    int Id,
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<bool>>;
