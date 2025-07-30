namespace StockDaddy.Application.DTOs;
public class ProductImageDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = "";
    public bool IsPrimary { get; set; }
}
