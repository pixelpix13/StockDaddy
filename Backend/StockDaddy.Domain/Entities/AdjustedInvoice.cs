namespace StockDaddy.Domain.Entities;

public class AdjustedInvoice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InvoiceId { get; set; }
    public decimal AdjustedTotalAmount { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public Guid AdjustedBy { get; set; }
    public DateTime AdjustedAt { get; set; } = DateTime.UtcNow;
    public bool IsVisibleToCustomer { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public Invoice? Invoice { get; set; }
    public User? AdjustedByUser { get; set; }
}
