using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Orders;

public class CreateOrderDto
{
    public PaymentMethod PaymentMethod { get; set; }
    public string? CouponCode { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
}
