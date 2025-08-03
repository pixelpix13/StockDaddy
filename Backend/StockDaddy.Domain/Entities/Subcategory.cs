namespace StockDaddy.Domain.Entities;

public class Subcategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StoreId { get; set; }
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }

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
