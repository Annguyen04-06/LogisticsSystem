namespace Logistics.Application.DTOs.Reports;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CancelledOrders { get; set; }
}
