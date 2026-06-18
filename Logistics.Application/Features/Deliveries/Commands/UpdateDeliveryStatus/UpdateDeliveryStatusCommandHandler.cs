using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Deliveries.Commands.UpdateDeliveryStatus;

public class UpdateDeliveryStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateDeliveryStatusCommand, ApiResponse<DeliveryDto>>
{
    public async Task<ApiResponse<DeliveryDto>> Handle(UpdateDeliveryStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Shipper and not UserRole.Admin)
        {
            return ApiResponse<DeliveryDto>.Fail("Only shipper or admin can update delivery status.");
        }

        var delivery = await context.Deliveries
            .FirstOrDefaultAsync(delivery => delivery.Id == request.DeliveryId, cancellationToken);

        if (delivery is null)
        {
            return ApiResponse<DeliveryDto>.Fail("Delivery does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Shipper && delivery.ShipperId != request.CurrentUserId)
        {
            return ApiResponse<DeliveryDto>.Fail("Shipper can only update assigned delivery.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == delivery.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<DeliveryDto>.Fail("Order does not exist.");
        }

        if (!Enum.TryParse<OrderStatus>(delivery.Status, out var currentStatus))
        {
            return ApiResponse<DeliveryDto>.Fail("Current delivery status is invalid.");
        }

        var newStatus = request.DeliveryStatus.Status;
        var isAllowed = currentStatus == OrderStatus.Confirmed && newStatus == OrderStatus.Shipping ||
            currentStatus == OrderStatus.Shipping && newStatus == OrderStatus.Delivered;

        if (!isAllowed)
        {
            return ApiResponse<DeliveryDto>.Fail("Delivery status update is not allowed.");
        }

        delivery.Status = newStatus.ToString();
        delivery.UpdatedAt = DateTime.UtcNow;
        Payment? codPaymentForTransaction = null;

        if (newStatus == OrderStatus.Shipping)
        {
            order.Status = OrderStatus.Shipping;
        }

        if (newStatus == OrderStatus.Delivered)
        {
            var payment = await context.Payments
                .Where(payment => payment.OrderId == order.Id)
                .OrderByDescending(payment => payment.CreatedAt)
                .ThenByDescending(payment => payment.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (order.PaymentMethod is PaymentMethod.Wallet or PaymentMethod.BankingDemo &&
                payment?.Status != PaymentStatus.Paid)
            {
                return ApiResponse<DeliveryDto>.Fail(
                    "Đơn hàng chưa được thanh toán. Không thể hoàn tất giao hàng.");
            }

            if (order.PaymentMethod == PaymentMethod.COD)
            {
                var wasPaid = payment?.Status == PaymentStatus.Paid;
                var paidAt = DateTime.UtcNow;

                if (payment is null)
                {
                    payment = new Payment
                    {
                        OrderId = order.Id,
                        UserId = order.CustomerId,
                        Amount = order.FinalAmount,
                        Method = PaymentMethod.COD,
                        Status = PaymentStatus.Paid,
                        PaidAt = paidAt
                    };

                    context.Payments.Add(payment);
                }
                else
                {
                    payment.UserId = order.CustomerId;
                    payment.Amount = order.FinalAmount;
                    payment.Method = PaymentMethod.COD;
                    payment.Status = PaymentStatus.Paid;
                    payment.PaidAt ??= paidAt;
                    payment.UpdatedAt = paidAt;
                }

                if (!wasPaid)
                {
                    codPaymentForTransaction = payment;
                }
            }

            order.Status = OrderStatus.Delivered;
            delivery.DeliveredAt = DateTime.UtcNow;
        }

        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        if (codPaymentForTransaction is not null)
        {
            context.PaymentTransactions.Add(new PaymentTransaction
            {
                PaymentId = codPaymentForTransaction.Id,
                UserId = order.CustomerId,
                OrderId = order.Id,
                TransactionCode = "COD",
                Amount = order.FinalAmount,
                Status = PaymentStatus.Paid,
                Note = "Thanh toán khi nhận hàng"
            });

            await context.SaveChangesAsync(cancellationToken);
        }

        var deliveryDto = await DeliveryDtoBuilder.BuildOneAsync(
            context,
            context.Deliveries.Where(item => item.Id == delivery.Id),
            cancellationToken);

        return ApiResponse<DeliveryDto>.Ok(deliveryDto!, "Delivery status updated successfully.");
    }
}
