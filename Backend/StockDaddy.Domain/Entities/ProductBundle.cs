namespace StockDaddy.Domain.Entities;

public class ProductBundle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public Tenant? Tenant { get; set; }
}
