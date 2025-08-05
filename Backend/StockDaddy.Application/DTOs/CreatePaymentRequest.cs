using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreatePaymentRequest
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaidAt { get; set; }
    public int ReceivedBy { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
}
