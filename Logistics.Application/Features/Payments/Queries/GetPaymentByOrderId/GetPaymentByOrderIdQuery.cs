using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdQuery : IRequest<ApiResponse<PaymentDto>>
{
    public int OrderId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetPaymentByOrderIdQuery(int orderId, int currentUserId, UserRole currentUserRole)
    {
        OrderId = orderId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
