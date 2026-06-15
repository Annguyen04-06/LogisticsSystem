using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Deliveries;

public class UpdateDeliveryStatusDto
{
    public OrderStatus Status { get; set; }
}
