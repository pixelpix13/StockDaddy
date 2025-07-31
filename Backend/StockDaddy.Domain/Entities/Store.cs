namespace StockDaddy.Domain.Entities;

public class Store
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Tenant? Tenant { get; set; }
    public ICollection<User>? Users { get; set; }
    public ICollection<Product>? Products { get; set; }
    public ICollection<Sale>? Sales { get; set; }
}
