namespace StockDaddy.Application.DTOs;

public class UpdateProductImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
