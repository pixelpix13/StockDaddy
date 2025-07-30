
namespace StockDaddy.Application.DTOs;
    
public class UpdateProductImageRequest
{
    public string ImageUrl { get; set; } = "";
    public bool IsPrimary { get; set; }
}
