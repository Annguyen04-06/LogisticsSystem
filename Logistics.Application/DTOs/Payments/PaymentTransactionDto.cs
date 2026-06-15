namespace Logistics.Application.DTOs.Payments;

public class PaymentTransactionDto
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
