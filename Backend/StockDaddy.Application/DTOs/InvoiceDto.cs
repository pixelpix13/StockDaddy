namespace StockDaddy.Application.DTOs;
public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "";
    public string? FileUrl { get; set; }
}
