namespace StockDaddy.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Store? Store { get; set; }
    public Tenant? Tenant { get; set; }
    public ICollection<Subcategory>? Subcategories { get; set; }
}
