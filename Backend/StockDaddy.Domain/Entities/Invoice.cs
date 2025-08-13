using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid? StoreId { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public string? FileUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    

    // Navigation
    public Sale? Sale { get; set; }
    public Store? Store { get; set; }
}
