namespace Logistics.Application.DTOs.Payments;

public class BankingQrDto
{
    public string BankCode { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string AddInfo { get; set; } = string.Empty;
    public string QrImageUrl { get; set; } = string.Empty;
}
