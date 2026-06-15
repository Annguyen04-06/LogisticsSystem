using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public int PaymentId { get; }
    public UserRole CurrentUserRole { get; }

    public RefundPaymentCommand(int paymentId, UserRole currentUserRole)
    {
        PaymentId = paymentId;
        CurrentUserRole = currentUserRole;
    }
}
