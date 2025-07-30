namespace StockDaddy.Domain.Entities;

public class StockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "in-stock"; // Could be Enum if needed
    public Guid UpdatedBy { get; set; }

    // Navigation
    public Product? Product { get; set; }
    public User? UpdatedByUser { get; set; }
}
