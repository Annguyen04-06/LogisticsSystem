using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Deliveries.Commands.AssignShipper;

public class AssignShipperCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AssignShipperCommand, ApiResponse<DeliveryDto>>
{
    public async Task<ApiResponse<DeliveryDto>> Handle(AssignShipperCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<DeliveryDto>.Fail("Only seller or admin can assign shipper.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<DeliveryDto>.Fail("Order does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Seller && order.SellerId != request.CurrentUserId)
        {
            return ApiResponse<DeliveryDto>.Fail("Seller can only assign shipper for their own order.");
        }

        if (order.Status != OrderStatus.Confirmed)
        {
            return ApiResponse<DeliveryDto>.Fail("Chỉ có thể gán người giao hàng cho đơn đã xác nhận.");
        }

        var shipper = await context.Users
            .FirstOrDefaultAsync(
                user => user.Id == request.AssignShipper.ShipperId,
                cancellationToken);

        if (shipper is null)
        {
            return ApiResponse<DeliveryDto>.Fail("Không tìm thấy người giao hàng.");
        }

        if (shipper.Role != UserRole.Shipper)
        {
            return ApiResponse<DeliveryDto>.Fail("Tài khoản được chọn không phải là người giao hàng.");
        }

        if (!shipper.IsActive)
        {
            return ApiResponse<DeliveryDto>.Fail("Tài khoản người giao hàng đã bị vô hiệu hóa.");
        }

        if (!shipper.IsApproved)
        {
            return ApiResponse<DeliveryDto>.Fail("Tài khoản người giao hàng chưa được phê duyệt.");
        }

        var existingDelivery = await context.Deliveries
            .FirstOrDefaultAsync(delivery => delivery.OrderId == order.Id, cancellationToken);

        if (existingDelivery is not null)
        {
            existingDelivery.ShipperId = shipper.Id;
            existingDelivery.Status = OrderStatus.Confirmed.ToString();
            existingDelivery.AssignedAt = DateTime.UtcNow;
            existingDelivery.DeliveredAt = null;
            existingDelivery.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existingDelivery = new Delivery
            {
                OrderId = order.Id,
                ShipperId = shipper.Id,
                Status = OrderStatus.Confirmed.ToString(),
                AssignedAt = DateTime.UtcNow
            };

            context.Deliveries.Add(existingDelivery);
        }

        order.ShipperId = shipper.Id;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var deliveryDto = await DeliveryDtoBuilder.BuildOneAsync(
            context,
            context.Deliveries.Where(delivery => delivery.Id == existingDelivery.Id),
            cancellationToken);

        return ApiResponse<DeliveryDto>.Ok(deliveryDto!, "Shipper assigned successfully.");
    }
}
