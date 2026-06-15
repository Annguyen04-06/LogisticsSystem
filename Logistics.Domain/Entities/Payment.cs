using Logistics.Domain.Common;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
}
