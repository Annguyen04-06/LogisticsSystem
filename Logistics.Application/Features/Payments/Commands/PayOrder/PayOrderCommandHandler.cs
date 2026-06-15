using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Commands.PayOrder;

public class PayOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<PayOrderCommand, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<PaymentDto>.Fail("Only customer can pay for orders.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.Payment.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<PaymentDto>.Fail("Order does not exist.");
        }

        if (order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<PaymentDto>.Fail("You can only pay for your own order.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return ApiResponse<PaymentDto>.Fail("Cancelled orders cannot be paid.");
        }

        var paidPaymentExists = await context.Payments
            .AnyAsync(
                payment => payment.OrderId == order.Id && payment.Status == PaymentStatus.Paid,
                cancellationToken);

        if (paidPaymentExists)
        {
            return ApiResponse<PaymentDto>.Fail("Order has already been paid.");
        }

        return request.Payment.Method switch
        {
            PaymentMethod.COD => await CreatePaymentAsync(
                order.Id,
                request.CurrentUserId,
                order.FinalAmount,
                PaymentMethod.COD,
                PaymentStatus.Pending,
                "COD",
                "COD payment pending",
                cancellationToken),

            PaymentMethod.Wallet => await PayByWalletAsync(order.Id, request.CurrentUserId, order.FinalAmount, cancellationToken),

            PaymentMethod.BankingDemo => await CreatePaymentAsync(
                order.Id,
                request.CurrentUserId,
                order.FinalAmount,
                PaymentMethod.BankingDemo,
                PaymentStatus.Paid,
                "BankingDemo",
                "Demo banking payment successful",
                cancellationToken),

            _ => ApiResponse<PaymentDto>.Fail("Payment method is invalid.")
        };
    }

    private async Task<ApiResponse<PaymentDto>> PayByWalletAsync(
        int orderId,
        int userId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.UserId == userId, cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<PaymentDto>.Fail("Wallet does not exist.");
        }

        if (wallet.Balance < amount)
        {
            return ApiResponse<PaymentDto>.Fail("Wallet balance is not enough.");
        }

        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        return await CreatePaymentAsync(
            orderId,
            userId,
            amount,
            PaymentMethod.Wallet,
            PaymentStatus.Paid,
            "WalletPayment",
            "Wallet payment successful",
            cancellationToken);
    }

    private async Task<ApiResponse<PaymentDto>> CreatePaymentAsync(
        int orderId,
        int userId,
        decimal amount,
        PaymentMethod method,
        PaymentStatus status,
        string transactionType,
        string note,
        CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Method = method,
            Status = status
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);

        context.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentId = payment.Id,
            TransactionCode = transactionType,
            Amount = amount,
            Status = status,
            Note = note
        });

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Ok(PaymentDtoMapper.ToPaymentDto(payment), "Payment processed successfully.");
    }
}
