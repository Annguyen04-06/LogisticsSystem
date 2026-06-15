namespace Logistics.Application.DTOs.Coupons;

public class ApplyCouponResultDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
}
