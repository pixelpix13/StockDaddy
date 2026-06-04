namespace StockDaddy.Application.DTOs;

public class UpdateRefundRequest
{
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Method { get; set; } = string.Empty;
}
