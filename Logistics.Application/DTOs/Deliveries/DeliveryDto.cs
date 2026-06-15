using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Deliveries;

public class DeliveryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ShipperId { get; set; }
    public string ShipperName { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
