using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Payments;

public class PaymentDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
