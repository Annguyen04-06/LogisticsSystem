using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class SellerRating : BaseEntity
{
    public int SellerId { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public bool IsLike { get; set; }
    public string Comment { get; set; } = string.Empty;
}
