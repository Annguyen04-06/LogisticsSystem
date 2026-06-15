using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Queries.GetMyPayments;

public class GetMyPaymentsQuery : IRequest<ApiResponse<List<PaymentDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetMyPaymentsQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
