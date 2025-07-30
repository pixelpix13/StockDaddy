namespace StockDaddy.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional: Navigation
    public ICollection<User>? Users { get; set; }
    public ICollection<Product>? Products { get; set; }
}
