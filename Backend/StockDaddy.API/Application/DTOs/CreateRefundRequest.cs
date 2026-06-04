namespace StockDaddy.Application.DTOs;

public class CreateRefundRequest
{
    public int ReturnId { get; set; }
    public int? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;
}
