using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaidAt { get; set; }
    public Guid ReceivedBy { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Invoice? Invoice { get; set; }
    public User? ReceivedByUser { get; set; }
}
