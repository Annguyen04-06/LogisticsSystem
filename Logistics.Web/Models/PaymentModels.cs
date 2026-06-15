using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logistics.Web.Models;

public class PaymentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class WalletDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
}

public class DepositWalletDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền nạp phải lớn hơn 0.")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

public class PayOrderDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập mã đơn hàng.")]
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = "Wallet";
}
