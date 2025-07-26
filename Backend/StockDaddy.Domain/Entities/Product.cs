namespace StockDaddy.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique Product ID
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }

    // Optional: Track timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
