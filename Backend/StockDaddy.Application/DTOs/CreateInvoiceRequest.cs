namespace StockDaddy.Application.DTOs;  
public class CreateInvoiceRequest
{
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? FileUrl { get; set; }
}
