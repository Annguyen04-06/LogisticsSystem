using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public int SellerId { get; set; }
    public bool IsActive { get; set; } = true;
}
