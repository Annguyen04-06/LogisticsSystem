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
            return ApiResponse<WalletDto>.Fail("Only customer can deposit wallet.");
        }

        if (request.Deposit.Amount <= 0)
        {
            return ApiResponse<WalletDto>.Fail("Deposit amount must be greater than 0.");
        }

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
            await context.SaveChangesAsync(cancellationToken);
        }

        wallet.Balance += request.Deposit.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<WalletDto>.Ok(PaymentDtoMapper.ToWalletDto(wallet), "Wallet deposited successfully.");
    }
}
