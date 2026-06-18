using Logistics.Domain.Common;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PaymentTransaction : BaseEntity
{
    public int? PaymentId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string Note { get; set; } = string.Empty;
}
