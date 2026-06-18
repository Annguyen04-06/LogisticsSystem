using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Commands.DepositWallet;

public class DepositWalletCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DepositWalletCommand, ApiResponse<WalletDto>>
{
    public async Task<ApiResponse<WalletDto>> Handle(DepositWalletCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<WalletDto>.Fail("Chỉ khách hàng mới có thể nạp ví.");
        }

        if (request.Deposit.Amount <= 0)
        {
            return ApiResponse<WalletDto>.Fail("Số tiền nạp phải lớn hơn 0.");
        }

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.UserId == request.CurrentUserId, cancellationToken);

        if (wallet is null)
        {
            wallet = new Wallet
            {
                UserId = request.CurrentUserId,
                Balance = 0
            };

            context.Wallets.Add(wallet);
        }

        wallet.Balance += request.Deposit.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        context.PaymentTransactions.Add(new PaymentTransaction
        {
            UserId = request.CurrentUserId,
            TransactionCode = "Deposit",
            Amount = request.Deposit.Amount,
            Status = PaymentStatus.Paid,
            Note = "Nạp ví",
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<WalletDto>.Ok(PaymentDtoMapper.ToWalletDto(wallet), "Nạp ví thành công.");
    }
}
