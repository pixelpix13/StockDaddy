namespace StockDaddy.Application.DTOs;

public class CreatePurchaseItemRequest
{
    public int PurchaseOrderId { get; set; }
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
