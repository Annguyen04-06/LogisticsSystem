namespace Logistics.Application.DTOs.Dashboard;

public class OrderStatisticsByMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalOrders { get; set; }
    public decimal Revenue { get; set; }
}
