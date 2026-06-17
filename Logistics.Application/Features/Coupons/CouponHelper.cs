using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;

namespace Logistics.Application.Features.Coupons;

internal static class CouponHelper
{
    public static CouponDto ToDto(Coupon coupon)
    {
        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            MinOrderAmount = coupon.MinOrderAmount,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            UsageLimit = coupon.UsageLimit,
            UsedCount = coupon.UsedCount,
            IsActive = coupon.IsActive
        };
    }

    public static string? ValidateCouponFields(
        DiscountType discountType,
        decimal discountValue,
        decimal minOrderAmount,
        DateTime startDate,
        DateTime endDate,
        int usageLimit)
    {
        if (!Enum.IsDefined(discountType))
        {
            return "Discount type is invalid.";
        }

        if (discountValue <= 0)
        {
            return "Discount value must be greater than 0.";
        }

        if (minOrderAmount < 0)
        {
            return "Minimum order amount must be greater than or equal to 0.";
        }

        if (usageLimit <= 0)
        {
            return "Usage limit must be greater than 0.";
        }

        if (endDate <= startDate)
        {
            return "End date must be greater than start date.";
        }

        if (discountType == DiscountType.Percent && discountValue > 100)
        {
            return "Percent discount value must be less than or equal to 100.";
        }

        return null;
    }

    public static string? ValidateCanApply(Coupon coupon, decimal orderAmount, DateTime currentTime)
    {
        if (!coupon.IsActive)
        {
            return "Mã giảm giá đã ngừng hoạt động.";
        }

        if (currentTime < coupon.StartDate || currentTime > coupon.EndDate)
        {
            return "Coupon is not valid at this time.";
        }

        if (coupon.UsedCount >= coupon.UsageLimit)
        {
            return "Coupon usage limit has been reached.";
        }

        if (orderAmount < coupon.MinOrderAmount)
        {
            return "Order amount does not meet coupon minimum amount.";
        }

        return null;
    }

    public static decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
    {
        var discountAmount = coupon.DiscountType == DiscountType.Percent
            ? orderAmount * coupon.DiscountValue / 100
            : coupon.DiscountValue;

        return Math.Min(discountAmount, orderAmount);
    }
}
