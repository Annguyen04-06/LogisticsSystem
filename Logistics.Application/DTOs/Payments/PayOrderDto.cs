using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Payments;

public class PayOrderDto
{
    public int OrderId { get; set; }
    public PaymentMethod Method { get; set; }
}
