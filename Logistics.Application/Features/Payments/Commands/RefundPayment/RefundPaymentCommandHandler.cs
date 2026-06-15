using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RefundPaymentCommand, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<PaymentDto>.Fail("Only admin can refund payment.");
        }

        var payment = await context.Payments
            .FirstOrDefaultAsync(payment => payment.Id == request.PaymentId, cancellationToken);

        if (payment is null)
        {
            return ApiResponse<PaymentDto>.Fail("Payment does not exist.");
        }

        if (payment.Status != PaymentStatus.Paid)
        {
            return ApiResponse<PaymentDto>.Fail("Only paid payments can be refunded.");
        }

        if (payment.Method == PaymentMethod.Wallet)
        {
            var wallet = await context.Wallets
                .FirstOrDefaultAsync(wallet => wallet.UserId == payment.UserId, cancellationToken);

            if (wallet is null)
            {
                return ApiResponse<PaymentDto>.Fail("Customer wallet does not exist.");
            }

            wallet.Balance += payment.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;
        }

        payment.Status = PaymentStatus.Refunded;
        payment.UpdatedAt = DateTime.UtcNow;

        context.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentId = payment.Id,
            TransactionCode = "Refund",
            Amount = payment.Amount,
            Status = PaymentStatus.Refunded,
            Note = "Payment refunded"
        });

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Ok(PaymentDtoMapper.ToPaymentDto(payment), "Payment refunded successfully.");
    }
}
