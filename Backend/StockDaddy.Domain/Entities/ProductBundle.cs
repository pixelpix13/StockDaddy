namespace StockDaddy.Domain.Entities;

public class ProductBundle
{
    public int Id { get; set; }
    public int? TenantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Optional Navigation
    public Tenant? Tenant { get; set; }
    public ICollection<BundleItem>? BundleItems { get; set; }
    public ICollection<BundleSaleItem>? BundleSaleItems { get; set; }
}
