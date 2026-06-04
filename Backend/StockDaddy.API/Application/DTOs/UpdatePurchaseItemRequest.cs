namespace StockDaddy.Application.DTOs;

public class UpdatePurchaseItemRequest
{
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
