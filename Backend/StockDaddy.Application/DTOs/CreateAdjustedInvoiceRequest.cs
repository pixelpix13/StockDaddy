namespace StockDaddy.Application.DTOs;

public class CreateAdjustedInvoiceRequest
{
    public Guid InvoiceId { get; set; }
    public decimal AdjustedTotalAmount { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public Guid AdjustedBy { get; set; }
    public bool IsVisibleToCustomer { get; set; }
}
