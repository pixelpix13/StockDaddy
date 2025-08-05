using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaidAt { get; set; }
    public int ReceivedBy { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
