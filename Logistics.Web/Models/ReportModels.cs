using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logistics.Web.Models;

public class RevenueReportDto
{
    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("deliveredOrders")]
    public int DeliveredOrders { get; set; }

    [JsonPropertyName("pendingOrders")]
    public int PendingOrders { get; set; }

    [JsonPropertyName("cancelledOrders")]
    public int CancelledOrders { get; set; }
}

public class TopProductReportDto
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("totalSold")]
    public int TotalSold { get; set; }

    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }
}

public class SellerRevenueReportDto
{
    [JsonPropertyName("sellerId")]
    public int SellerId { get; set; }

    [JsonPropertyName("sellerName")]
    public string SellerName { get; set; } = string.Empty;

    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("deliveredOrders")]
    public int DeliveredOrders { get; set; }
}

public class InvoiceDownloadForm
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập mã đơn hàng.")]
    public int OrderId { get; set; }
}
