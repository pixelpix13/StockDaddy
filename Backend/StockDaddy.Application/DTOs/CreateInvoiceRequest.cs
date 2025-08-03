using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreateInvoiceRequest
{
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid? StoreId { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public string? FileUrl { get; set; }
}
