using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Commands.DepositWallet;

public class DepositWalletCommand : IRequest<ApiResponse<WalletDto>>
{
    public DepositWalletDto Deposit { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public DepositWalletCommand(DepositWalletDto deposit, int currentUserId, UserRole currentUserRole)
    {
        Deposit = deposit;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
