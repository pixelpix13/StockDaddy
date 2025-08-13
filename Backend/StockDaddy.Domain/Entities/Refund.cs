namespace StockDaddy.Domain.Entities;

public class Refund
{
    public int Id { get; set; }

    public int ReturnId { get; set; }
    public int? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    

    // Navigation
    public Return? Return { get; set; }
    public Store? Store { get; set; }
}
