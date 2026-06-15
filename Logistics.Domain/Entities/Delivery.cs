using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Delivery : BaseEntity
{
    public int OrderId { get; set; }
    public int ShipperId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
}
