namespace StockDaddy.Application.DTOs;

public class UpdateAdjustedInvoiceRequest
{
    public decimal AdjustedTotalAmount { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public bool IsVisibleToCustomer { get; set; }
}
