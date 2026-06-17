using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Payments.Queries.GetOrderBankingQr;

public class GetOrderBankingQrQuery : IRequest<ApiResponse<BankingQrDto>>
{
    public int OrderId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }
    public string BankCode { get; }
    public string AccountNumber { get; }
    public string AccountName { get; }

    public GetOrderBankingQrQuery(
        int orderId,
        int currentUserId,
        UserRole currentUserRole,
        string bankCode,
        string accountNumber,
        string accountName)
    {
        OrderId = orderId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
        BankCode = bankCode;
        AccountNumber = accountNumber;
        AccountName = accountName;
    }
}
