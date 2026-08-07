using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

/// <summary>
/// Tracks money to collect from customers or pay to suppliers on credit, with due dates.
/// </summary>
public class CreditLedger
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public CreditPartyType PartyType { get; set; }
    public CreditStatus Status { get; set; } = CreditStatus.Pending;

    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public int? SaleId { get; set; }
    public int? PurchaseOrderId { get; set; }

    public string PartyName { get; set; } = string.Empty;
    public string? PartyPhone { get; set; }
    public string? PartyEmail { get; set; }
    public string? PartyAddress { get; set; }

    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Tenant? Tenant { get; set; }
    public Customer? Customer { get; set; }
    public Supplier? Supplier { get; set; }
    public Sale? Sale { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
}
