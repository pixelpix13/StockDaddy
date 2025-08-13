namespace StockDaddy.Domain.Entities;

public class Subcategory
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int TenantId { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Store? Store { get; set; }
    public Tenant? Tenant { get; set; }
    public Category? Category { get; set; }
    public ICollection<Product>? Products { get; set; }
}
