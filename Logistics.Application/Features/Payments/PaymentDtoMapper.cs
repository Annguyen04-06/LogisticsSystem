using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Entities;

namespace Logistics.Application.Features.Payments;

internal static class PaymentDtoMapper
{
    public static PaymentDto ToPaymentDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            UserId = payment.UserId,
            Amount = payment.Amount,
            Method = payment.Method,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt
        };
    }

    public static WalletDto ToWalletDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Balance = wallet.Balance
        };
    }

    public static PaymentTransactionDto ToTransactionDto(PaymentTransaction transaction)
    {
        return new PaymentTransactionDto
        {
            Id = transaction.Id,
            PaymentId = transaction.PaymentId,
            OrderId = transaction.OrderId,
            Amount = transaction.Amount,
            Type = transaction.TransactionCode,
            Note = transaction.Note,
            Status = transaction.Status.ToString(),
            CreatedAt = transaction.CreatedAt
        };
    }
}
