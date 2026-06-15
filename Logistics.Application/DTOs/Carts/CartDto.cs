namespace Logistics.Application.DTOs.Carts;

public class CartDto
{
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
}
