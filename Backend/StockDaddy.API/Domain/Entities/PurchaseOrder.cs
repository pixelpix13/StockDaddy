using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }

    public int TenantId { get; set; }
    public int SupplierId { get; set; }
    public int StoreId { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDelivery { get; set; }

    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;

    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Tenant? Tenant { get; set; }
    public Supplier? Supplier { get; set; }
    public Store? Store { get; set; }
}
