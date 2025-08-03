namespace StockDaddy.Domain.Entities;

public class ProductBundle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }

    // Optional Navigation
    public Tenant? Tenant { get; set; }
    public ICollection<BundleItem>? BundleItems { get; set; }
    public ICollection<BundleSaleItem>? BundleSaleItems { get; set; }
}
