namespace StockDaddy.Application.DTOs;

public class ProductBundleDto
{
    public int Id { get; set; }
    public int? TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
