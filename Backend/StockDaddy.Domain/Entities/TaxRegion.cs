namespace StockDaddy.Domain.Entities;

public class TaxRegion
{
    public int Id { get; set; }

    public int TenantId { get; set; }
    public int? StoreId { get; set; } // Optional override

    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
}
