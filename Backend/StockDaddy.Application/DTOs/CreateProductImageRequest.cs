namespace StockDaddy.Application.DTOs;

public class CreateProductImageRequest
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
