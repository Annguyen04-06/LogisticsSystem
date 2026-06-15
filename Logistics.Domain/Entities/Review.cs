using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public int? OrderId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
