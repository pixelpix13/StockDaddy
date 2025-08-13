using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? StoreId { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? FileUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
