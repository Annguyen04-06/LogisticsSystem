using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class StockTransaction : BaseEntity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
