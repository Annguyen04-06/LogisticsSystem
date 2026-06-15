namespace Logistics.Application.DTOs.Dashboard;

public class OrderStatisticsByDateDto
{
    public DateTime Date { get; set; }
    public int TotalOrders { get; set; }
    public decimal Revenue { get; set; }
}
