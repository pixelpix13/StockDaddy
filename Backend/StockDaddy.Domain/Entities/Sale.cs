namespace StockDaddy.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? CustomerId { get; set; }  // Optional
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public Guid SoldBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // Navigation
    public Customer? Customer { get; set; }
    public User? Seller { get; set; }
}
