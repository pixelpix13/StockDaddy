namespace StockDaddy.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }


    // Optional navigation
    public ICollection<User>? Users { get; set; }
    public ICollection<Product>? Products { get; set; }
    public ICollection<Store>? Stores { get; set; }
}
