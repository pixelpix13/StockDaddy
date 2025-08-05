namespace StockDaddy.Domain.Entities;

public class ProductTag
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Tag { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;    

    // Navigation
    public Product? Product { get; set; }
}
