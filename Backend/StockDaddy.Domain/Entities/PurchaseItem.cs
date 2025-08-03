namespace StockDaddy.Domain.Entities;

public class PurchaseItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PurchaseOrderId { get; set; }
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    

    // Navigation
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}
