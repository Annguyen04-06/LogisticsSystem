using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logistics.Web.Models;

public class AdminDashboardDto
{
    [JsonPropertyName("totalUsers")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("totalCustomers")]
    public int TotalCustomers { get; set; }

    [JsonPropertyName("totalSellers")]
    public int TotalSellers { get; set; }

    [JsonPropertyName("totalShippers")]
    public int TotalShippers { get; set; }

    [JsonPropertyName("totalProducts")]
    public int TotalProducts { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("pendingOrders")]
    public int PendingOrders { get; set; }

    [JsonPropertyName("confirmedOrders")]
    public int ConfirmedOrders { get; set; }

    [JsonPropertyName("shippingOrders")]
    public int ShippingOrders { get; set; }

    [JsonPropertyName("deliveredOrders")]
    public int DeliveredOrders { get; set; }

    [JsonPropertyName("cancelledOrders")]
    public int CancelledOrders { get; set; }

    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("totalTickets")]
    public int TotalTickets { get; set; }

    [JsonPropertyName("openTickets")]
    public int OpenTickets { get; set; }

    [JsonPropertyName("closedTickets")]
    public int ClosedTickets { get; set; }
}

public class OrderStatisticsByDateDto
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("revenue")]
    public decimal Revenue { get; set; }
}

public class OrderStatisticsByMonthDto
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("revenue")]
    public decimal Revenue { get; set; }
}

public class CouponDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("discountType")]
    public string DiscountType { get; set; } = "Percent";

    [JsonPropertyName("discountValue")]
    public decimal DiscountValue { get; set; }

    [JsonPropertyName("minOrderAmount")]
    public decimal MinOrderAmount { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("usageLimit")]
    public int UsageLimit { get; set; }

    [JsonPropertyName("usedCount")]
    public int UsedCount { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}

public class CreateCouponDto
{
    [Required(ErrorMessage = "Vui lòng nhập mã giảm giá.")]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("discountType")]
    public string DiscountType { get; set; } = "Percent";

    [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
    [JsonPropertyName("discountValue")]
    public decimal DiscountValue { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Đơn tối thiểu không được âm.")]
    [JsonPropertyName("minOrderAmount")]
    public decimal MinOrderAmount { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

    [Range(1, int.MaxValue, ErrorMessage = "Số lượt dùng phải lớn hơn 0.")]
    [JsonPropertyName("usageLimit")]
    public int UsageLimit { get; set; } = 1;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class UpdateCouponDto
{
    [JsonPropertyName("discountType")]
    public string DiscountType { get; set; } = "Percent";

    [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
    [JsonPropertyName("discountValue")]
    public decimal DiscountValue { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Đơn tối thiểu không được âm.")]
    [JsonPropertyName("minOrderAmount")]
    public decimal MinOrderAmount { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

    [Range(1, int.MaxValue, ErrorMessage = "Số lượt dùng phải lớn hơn 0.")]
    [JsonPropertyName("usageLimit")]
    public int UsageLimit { get; set; } = 1;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}
