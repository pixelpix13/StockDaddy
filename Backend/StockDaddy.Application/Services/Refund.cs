namespace StockDaddy.Domain.Entities;

public class Refund
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReturnId { get; set; }
    public Guid? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Return? Return { get; set; }
    public Store? Store { get; set; }
}
