namespace Logistics.Application.DTOs.Dashboard;

public class SellerDashboardDto
{
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int ShippingOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalLikes { get; set; }
    public int TotalDislikes { get; set; }
    public decimal TrustPercent { get; set; }
}
