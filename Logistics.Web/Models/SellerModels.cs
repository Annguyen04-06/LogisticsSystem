using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logistics.Web.Models;

public class SellerDashboardDto
{
    [JsonPropertyName("sellerId")]
    public int SellerId { get; set; }

    [JsonPropertyName("sellerName")]
    public string SellerName { get; set; } = string.Empty;

    [JsonPropertyName("totalProducts")]
    public int TotalProducts { get; set; }

    [JsonPropertyName("totalOrders")]
    public int TotalOrders { get; set; }

    [JsonPropertyName("pendingOrders")]
    public int PendingOrders { get; set; }

    [JsonPropertyName("confirmedOrders")]
    public int ConfirmedOrders { get; set; }

    [JsonPropertyName("shippingOrders")]
    public int ShippingOrders { get; set; }

    [JsonPropertyName("deliveredOrders")]
    public int DeliveredOrders { get; set; }

    [JsonPropertyName("cancelledOrders")]
    public int CancelledOrders { get; set; }

    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("totalLikes")]
    public int TotalLikes { get; set; }

    [JsonPropertyName("totalDislikes")]
    public int TotalDislikes { get; set; }

    [JsonPropertyName("trustPercent")]
    public decimal TrustPercent { get; set; }
}

public class CreateProductDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm.")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class AssignShipperDto
{
    [JsonPropertyName("shipperId")]
    public int ShipperId { get; set; }
}

public class AvailableShipperDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }
}

public class UpdateDeliveryStatusDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class DeliveryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("shipperId")]
    public int ShipperId { get; set; }

    [JsonPropertyName("shipperName")]
    public string ShipperName { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("assignedAt")]
    public DateTime AssignedAt { get; set; }

    [JsonPropertyName("deliveredAt")]
    public DateTime? DeliveredAt { get; set; }
}
