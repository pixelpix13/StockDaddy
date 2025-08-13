using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class UpdateInvoiceRequest
{
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? FileUrl { get; set; }
}
