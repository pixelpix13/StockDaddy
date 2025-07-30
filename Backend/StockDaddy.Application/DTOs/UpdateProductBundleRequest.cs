namespace StockDaddy.Application.DTOs;
public class UpdateProductBundleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
