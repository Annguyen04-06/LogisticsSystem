using Logistics.Domain.Common;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public int SellerId { get; set; }
    public int? ShipperId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
