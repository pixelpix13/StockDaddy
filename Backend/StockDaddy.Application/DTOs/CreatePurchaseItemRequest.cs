namespace StockDaddy.Application.DTOs;

public class CreatePurchaseItemRequest
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
