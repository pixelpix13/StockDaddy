namespace StockDaddy.Application.DTOs;
public class UpdateInvoiceRequest
{
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public string? FileUrl { get; set; }
}
