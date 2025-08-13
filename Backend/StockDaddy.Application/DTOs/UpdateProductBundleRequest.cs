namespace StockDaddy.Application.DTOs;

public class UpdateProductBundleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
