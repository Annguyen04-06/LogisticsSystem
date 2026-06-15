using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Queries.GetMyWallet;

public class GetMyWalletQuery : IRequest<ApiResponse<WalletDto>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetMyWalletQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
