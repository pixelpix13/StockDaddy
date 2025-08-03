using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreatePaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PaidAt { get; set; }
    public Guid ReceivedBy { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
}
