using System.Globalization;
using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Queries.GetOrderBankingQr;

public class GetOrderBankingQrQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrderBankingQrQuery, ApiResponse<BankingQrDto>>
{
    public async Task<ApiResponse<BankingQrDto>> Handle(
        GetOrderBankingQrQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<BankingQrDto>.Fail("Bạn không có quyền thanh toán đơn hàng này.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<BankingQrDto>.Fail("Không tìm thấy đơn hàng.");
        }

        if (order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<BankingQrDto>.Fail("Bạn không có quyền thanh toán đơn hàng này.");
        }

        if (order.PaymentMethod != PaymentMethod.BankingDemo)
        {
            return ApiResponse<BankingQrDto>.Fail("Đơn hàng không sử dụng phương thức chuyển khoản giả lập.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return ApiResponse<BankingQrDto>.Fail("Không thể thanh toán đơn hàng đã hủy.");
        }

        var paidPaymentExists = await context.Payments
            .AnyAsync(
                payment => payment.OrderId == order.Id && payment.Status == PaymentStatus.Paid,
                cancellationToken);

        if (paidPaymentExists)
        {
            return ApiResponse<BankingQrDto>.Fail("Đơn hàng đã được thanh toán.");
        }

        var amount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount;
        var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        var addInfo = $"PAY_ORDER_{order.Id}*USER*{request.CurrentUserId}_{timestamp}";
        var amountText = amount.ToString("0", CultureInfo.InvariantCulture);
        var qrImageUrl =
            $"https://img.vietqr.io/image/{Uri.EscapeDataString(request.BankCode)}-{Uri.EscapeDataString(request.AccountNumber)}-compact2.png" +
            $"?amount={Uri.EscapeDataString(amountText)}" +
            $"&addInfo={Uri.EscapeDataString(addInfo)}" +
            $"&accountName={Uri.EscapeDataString(request.AccountName)}";

        var qr = new BankingQrDto
        {
            BankCode = request.BankCode,
            AccountNumber = request.AccountNumber,
            AccountName = request.AccountName,
            Amount = amount,
            AddInfo = addInfo,
            QrImageUrl = qrImageUrl
        };

        return ApiResponse<BankingQrDto>.Ok(qr, "Tạo mã QR thanh toán đơn hàng thành công.");
    }
}
