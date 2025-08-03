namespace StockDaddy.Application.DTOs;

public class CreateRefundRequest
{
    public Guid ReturnId { get; set; }
    public Guid? StoreId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;
}
