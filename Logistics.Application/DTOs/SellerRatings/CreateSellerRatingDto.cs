namespace Logistics.Application.DTOs.SellerRatings;

public class CreateSellerRatingDto
{
    public int OrderId { get; set; }
    public int SellerId { get; set; }
    public bool IsLike { get; set; }
    public string Comment { get; set; } = string.Empty;
}
