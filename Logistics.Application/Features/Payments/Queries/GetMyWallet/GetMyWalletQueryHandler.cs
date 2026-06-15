using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Queries.GetMyWallet;

public class GetMyWalletQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyWalletQuery, ApiResponse<WalletDto>>
{
    public async Task<ApiResponse<WalletDto>> Handle(GetMyWalletQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<WalletDto>.Fail("Only customer can view wallet.");
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

        return ApiResponse<WalletDto>.Ok(PaymentDtoMapper.ToWalletDto(wallet));
    }
}
