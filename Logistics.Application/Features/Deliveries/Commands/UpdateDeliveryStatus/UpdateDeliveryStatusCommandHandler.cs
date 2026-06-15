using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Interfaces;
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

        if (newStatus == OrderStatus.Shipping)
        {
            order.Status = OrderStatus.Shipping;
        }

        if (newStatus == OrderStatus.Delivered)
        {
            order.Status = OrderStatus.Delivered;
            delivery.DeliveredAt = DateTime.Now;
        }

        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var deliveryDto = await DeliveryDtoBuilder.BuildOneAsync(
            context,
            context.Deliveries.Where(item => item.Id == delivery.Id),
            cancellationToken);

        return ApiResponse<DeliveryDto>.Ok(deliveryDto!, "Delivery status updated successfully.");
    }
}
