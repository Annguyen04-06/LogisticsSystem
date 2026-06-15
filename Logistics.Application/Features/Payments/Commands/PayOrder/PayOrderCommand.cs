using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Commands.PayOrder;

public class PayOrderCommand : IRequest<ApiResponse<PaymentDto>>
{
    public PayOrderDto Payment { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public PayOrderCommand(PayOrderDto payment, int currentUserId, UserRole currentUserRole)
    {
        Payment = payment;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
