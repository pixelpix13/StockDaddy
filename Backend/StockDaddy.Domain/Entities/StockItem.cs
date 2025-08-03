using StockDaddy.Domain.Enums;
namespace StockDaddy.Domain.Entities;

public class StockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Guid? StoreId { get; set; }
    public int Quantity { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public StockStatus Status { get; set; } = StockStatus.InStock;

    public Guid? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;


    // Navigation properties

    public Product? Product { get; set; }
    public Store? Store { get; set; }
    public User? UpdatedByUser { get; set; }
}
