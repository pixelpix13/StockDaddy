namespace StockDaddy.Domain.Entities;

public class TaxRegion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; } // Optional override

    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
}
