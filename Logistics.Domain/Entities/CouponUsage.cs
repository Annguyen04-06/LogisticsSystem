using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class CouponUsage : BaseEntity
{
    public int CouponId { get; set; }
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public decimal DiscountAmount { get; set; }
}
