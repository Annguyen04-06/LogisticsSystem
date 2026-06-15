using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Coupons;

public class UpdateCouponDto
{
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; }
    public bool IsActive { get; set; }
}
