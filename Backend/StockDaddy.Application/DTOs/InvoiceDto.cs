using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid? StoreId { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? FileUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
