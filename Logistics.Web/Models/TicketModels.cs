using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logistics.Web.Models;

public class SupportTicketDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("sellerId")]
    public int? SellerId { get; set; }

    [JsonPropertyName("sellerName")]
    public string? SellerName { get; set; }

    [JsonPropertyName("orderId")]
    public int? OrderId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("replies")]
    public List<TicketReplyDto> Replies { get; set; } = [];
}

public class TicketReplyDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class CreateTicketDto
{
    [JsonPropertyName("sellerId")]
    public int? SellerId { get; set; }

    [JsonPropertyName("orderId")]
    public int? OrderId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class ReplyTicketDto
{
    [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi.")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class UpdateTicketStatusDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
