namespace StockDaddy.Application.DTOs;
public class CreateProductBundleRequest
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
