namespace Logistics.Application.DTOs.Reports;

public class TopSellerReportDto
{
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int TotalProductsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}
