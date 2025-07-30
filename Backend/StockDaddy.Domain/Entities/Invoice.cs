namespace StockDaddy.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Unpaid"; // Default status
    public string? FileUrl { get; set; } // Optional

    public Sale? Sale { get; set; }
}
