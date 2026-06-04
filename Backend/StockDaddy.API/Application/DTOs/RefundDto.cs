namespace StockDaddy.Application.DTOs;

public class RefundDto
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public int? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
