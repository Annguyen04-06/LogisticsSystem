using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Queries.GetWalletTransactions;

public class GetWalletTransactionsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetWalletTransactionsQuery, ApiResponse<List<WalletTransactionDto>>>
{
    public async Task<ApiResponse<List<WalletTransactionDto>>> Handle(
        GetWalletTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<WalletTransactionDto>>.Fail(
                "Chỉ khách hàng mới có thể xem lịch sử giao dịch ví.");
        }

        var transactions = await context.PaymentTransactions
            .Where(transaction =>
                transaction.UserId == request.CurrentUserId &&
                (transaction.TransactionCode == "Deposit" ||
                 transaction.TransactionCode == "Payment" ||
                 transaction.TransactionCode == "Refund" &&
                 context.Payments.Any(payment =>
                     payment.Id == transaction.PaymentId &&
                     (payment.Method == PaymentMethod.Wallet ||
                      payment.Method == PaymentMethod.BankingDemo))))
            .OrderByDescending(transaction => transaction.CreatedAt)
            .ThenByDescending(transaction => transaction.Id)
            .Select(transaction => new WalletTransactionDto
            {
                Id = transaction.Id,
                Type = transaction.TransactionCode,
                Amount = transaction.Amount,
                Note = transaction.Note,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status == PaymentStatus.Pending
                    ? "Pending"
                    : transaction.Status == PaymentStatus.Failed
                        ? "Failed"
                        : "Completed",
                OrderId = transaction.OrderId
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<WalletTransactionDto>>.Ok(transactions);
    }
}
