namespace StockDaddy.Domain.Entities;

public class BundleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BundleId { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal EffectiveUnitPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public ProductBundle? ProductBundle { get; set; }
    public Product? Product { get; set; }
}
