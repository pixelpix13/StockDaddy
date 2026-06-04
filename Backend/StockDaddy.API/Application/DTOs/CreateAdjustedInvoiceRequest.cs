namespace StockDaddy.Application.DTOs;

public class CreateAdjustedInvoiceRequest
{
    public int InvoiceId { get; set; }
    public decimal AdjustedTotalAmount { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public int AdjustedBy { get; set; }
    public bool IsVisibleToCustomer { get; set; }
}
