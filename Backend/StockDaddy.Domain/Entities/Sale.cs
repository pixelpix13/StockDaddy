using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
    public Customer? Customer { get; set; }
    public User? SoldByUser { get; set; }
}
