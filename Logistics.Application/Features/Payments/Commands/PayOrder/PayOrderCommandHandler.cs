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
            return ApiResponse<PaymentDto>.Fail("Chỉ khách hàng mới có thể thanh toán đơn hàng.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.Payment.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<PaymentDto>.Fail("Không tìm thấy đơn hàng.");
        }

        if (order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<PaymentDto>.Fail("Bạn chỉ có thể thanh toán đơn hàng của mình.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return ApiResponse<PaymentDto>.Fail("Không thể thanh toán đơn hàng đã hủy.");
        }

        var payments = await context.Payments
            .Where(payment => payment.OrderId == order.Id)
            .OrderByDescending(payment => payment.CreatedAt)
            .ThenByDescending(payment => payment.Id)
            .ToListAsync(cancellationToken);

        if (payments.Any(payment => payment.Status == PaymentStatus.Paid))
        {
            return ApiResponse<PaymentDto>.Fail("Đơn hàng đã được thanh toán.");
        }

        if (payments.Any(payment => payment.Status == PaymentStatus.Refunded))
        {
            return ApiResponse<PaymentDto>.Fail("Đơn hàng đã được hoàn tiền và không thể thanh toán lại.");
        }

        if (request.Payment.Method != order.PaymentMethod)
        {
            return ApiResponse<PaymentDto>.Fail("Phương thức thanh toán không khớp với đơn hàng.");
        }

        var payment = payments.FirstOrDefault();

        return order.PaymentMethod switch
        {
            PaymentMethod.COD => await KeepCodPaymentPendingAsync(
                payment,
                order.Id,
                request.CurrentUserId,
                order.FinalAmount,
                cancellationToken),

            PaymentMethod.Wallet => await PayByWalletAsync(
                payment,
                order.Id,
                request.CurrentUserId,
                order.FinalAmount,
                cancellationToken),

            PaymentMethod.BankingDemo => await CompletePaymentAsync(
                payment,
                order.Id,
                request.CurrentUserId,
                order.FinalAmount,
                PaymentMethod.BankingDemo,
                "BankingDemo",
                "Thanh toán chuyển khoản giả lập thành công",
                cancellationToken),

            _ => ApiResponse<PaymentDto>.Fail("Phương thức thanh toán không hợp lệ.")
        };
    }

    private async Task<ApiResponse<PaymentDto>> KeepCodPaymentPendingAsync(
        Payment? payment,
        int orderId,
        int userId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        payment ??= CreatePayment(orderId, userId, amount, PaymentMethod.COD);
        payment.UserId = userId;
        payment.Amount = amount;
        payment.Method = PaymentMethod.COD;
        payment.Status = PaymentStatus.Pending;
        payment.PaidAt = null;
        payment.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Ok(
            PaymentDtoMapper.ToPaymentDto(payment),
            "Đơn hàng COD sẽ được thanh toán khi giao hàng thành công.");
    }

    private async Task<ApiResponse<PaymentDto>> PayByWalletAsync(
        Payment? payment,
        int orderId,
        int userId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.UserId == userId, cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<PaymentDto>.Fail("Không tìm thấy ví điện tử.");
        }

        if (wallet.Balance < amount)
        {
            return ApiResponse<PaymentDto>.Fail("Số dư ví không đủ để thanh toán.");
        }

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        var response = await CompletePaymentAsync(
            payment,
            orderId,
            userId,
            amount,
            PaymentMethod.Wallet,
            "Payment",
            $"Thanh toán đơn hàng #{orderId}",
            cancellationToken);

        if (!response.Success)
        {
            await transaction.RollbackAsync(cancellationToken);
            return response;
        }

        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    private async Task<ApiResponse<PaymentDto>> CompletePaymentAsync(
        Payment? payment,
        int orderId,
        int userId,
        decimal amount,
        PaymentMethod method,
        string transactionCode,
        string note,
        CancellationToken cancellationToken)
    {
        var isNewPayment = payment is null;
        payment ??= CreatePayment(orderId, userId, amount, method);
        var paidAt = DateTime.UtcNow;

        payment.UserId = userId;
        payment.Amount = amount;
        payment.Method = method;
        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = paidAt;
        payment.UpdatedAt = paidAt;

        if (isNewPayment)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        context.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentId = payment.Id,
            UserId = userId,
            OrderId = orderId,
            TransactionCode = transactionCode,
            Amount = amount,
            Status = PaymentStatus.Paid,
            Note = note,
            CreatedAt = paidAt
        });

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Ok(
            PaymentDtoMapper.ToPaymentDto(payment),
            "Thanh toán đơn hàng thành công.");
    }

    private Payment CreatePayment(int orderId, int userId, decimal amount, PaymentMethod method)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Method = method,
            Status = PaymentStatus.Pending
        };

        context.Payments.Add(payment);
        return payment;
    }
}
