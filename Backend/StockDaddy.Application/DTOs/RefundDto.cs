namespace StockDaddy.Application.DTOs;

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid ReturnId { get; set; }
    public Guid? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
