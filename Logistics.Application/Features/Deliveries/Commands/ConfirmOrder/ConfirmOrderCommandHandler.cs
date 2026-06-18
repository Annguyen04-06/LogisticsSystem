using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Features.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Deliveries.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ConfirmOrderCommand, ApiResponse<OrderDto>>
{
    public async Task<ApiResponse<OrderDto>> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<OrderDto>.Fail("Chỉ người bán hoặc quản trị viên mới có thể xác nhận đơn hàng.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<OrderDto>.Fail("Không tìm thấy đơn hàng.");
        }

        if (request.CurrentUserRole == UserRole.Seller && order.SellerId != request.CurrentUserId)
        {
            return ApiResponse<OrderDto>.Fail("Người bán chỉ có thể xác nhận đơn hàng của mình.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return ApiResponse<OrderDto>.Fail("Không thể xác nhận đơn hàng đã hủy.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            return ApiResponse<OrderDto>.Fail("Chỉ đơn hàng đang chờ xác nhận mới có thể được xác nhận.");
        }

        var payment = await context.Payments
            .Where(payment => payment.OrderId == order.Id)
            .OrderByDescending(payment => payment.CreatedAt)
            .ThenByDescending(payment => payment.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (payment?.Status == PaymentStatus.Refunded)
        {
            return ApiResponse<OrderDto>.Fail("Không thể xác nhận đơn hàng đã được hoàn tiền.");
        }

        if (order.PaymentMethod is PaymentMethod.Wallet or PaymentMethod.BankingDemo &&
            payment?.Status != PaymentStatus.Paid)
        {
            return ApiResponse<OrderDto>.Fail(
                "Đơn hàng chưa được thanh toán. Vui lòng yêu cầu khách hàng thanh toán trước khi xác nhận.");
        }

        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var orderDto = await OrderDtoBuilder.BuildOneAsync(
            context,
            context.Orders.Where(item => item.Id == order.Id),
            cancellationToken);

        return ApiResponse<OrderDto>.Ok(orderDto!, "Xác nhận đơn hàng thành công.");
    }
}
