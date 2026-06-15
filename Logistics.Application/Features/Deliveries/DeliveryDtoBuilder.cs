using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Deliveries;

internal static class DeliveryDtoBuilder
{
    public static async Task<List<DeliveryDto>> BuildListAsync(
        IApplicationDbContext context,
        IQueryable<Delivery> deliveriesQuery,
        CancellationToken cancellationToken)
    {
        return await (
            from delivery in deliveriesQuery
            join shipper in context.Users on delivery.ShipperId equals shipper.Id
            orderby delivery.AssignedAt descending
            select new DeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                ShipperId = delivery.ShipperId,
                ShipperName = shipper.FullName,
                Status = ParseStatus(delivery.Status),
                AssignedAt = delivery.AssignedAt,
                DeliveredAt = delivery.DeliveredAt
            }).ToListAsync(cancellationToken);
    }

    public static async Task<DeliveryDto?> BuildOneAsync(
        IApplicationDbContext context,
        IQueryable<Delivery> deliveriesQuery,
        CancellationToken cancellationToken)
    {
        var deliveries = await BuildListAsync(context, deliveriesQuery, cancellationToken);
        return deliveries.FirstOrDefault();
    }

    private static OrderStatus ParseStatus(string status)
    {
        return Enum.TryParse<OrderStatus>(status, out var parsedStatus)
            ? parsedStatus
            : OrderStatus.Pending;
    }
}
