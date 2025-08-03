namespace StockDaddy.Application.DTOs;

public class UpdateBundleSaleItemRequest
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
