namespace StockDaddy.Domain.Entities;

/// <summary>
/// Wholesale / B2B buyer organization (distinct from retail walk-in customers).
/// </summary>
public class Company
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int StoreId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Gstin { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
}
