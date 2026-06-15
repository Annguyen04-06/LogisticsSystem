namespace Logistics.Application.DTOs.Reports;

public class SellerRevenueReportDto
{
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
}
