using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Queries.GetWalletTransactions;

public class GetWalletTransactionsQuery(
    int currentUserId,
    UserRole currentUserRole) : IRequest<ApiResponse<List<WalletTransactionDto>>>
{
    public int CurrentUserId { get; } = currentUserId;
    public UserRole CurrentUserRole { get; } = currentUserRole;
}
