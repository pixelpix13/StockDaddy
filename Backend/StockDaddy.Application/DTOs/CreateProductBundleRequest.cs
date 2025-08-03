namespace StockDaddy.Application.DTOs;

public class CreateProductBundleRequest
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
