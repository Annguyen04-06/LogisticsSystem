namespace Logistics.Application.DTOs.SellerRatings;

public class SellerReputationDto
{
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int TotalLikes { get; set; }
    public int TotalDislikes { get; set; }
    public int ReputationScore { get; set; }
    public decimal TrustPercent { get; set; }
}
