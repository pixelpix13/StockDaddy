namespace StockDaddy.Domain.Entities;

public class ProductRestockAlert
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int VariantId { get; set; }

    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Optional: enum string for now

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;    
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public Product? Product { get; set; }
    public Store? Store { get; set; }
    public ProductVariant? Variant { get; set; }
}
