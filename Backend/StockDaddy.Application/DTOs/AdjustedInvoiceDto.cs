namespace StockDaddy.Application.DTOs;

public class AdjustedInvoiceDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal AdjustedTotalAmount { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public Guid AdjustedBy { get; set; }
    public DateTime AdjustedAt { get; set; }
    public bool IsVisibleToCustomer { get; set; }
}
