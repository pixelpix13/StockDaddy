namespace StockDaddy.Application.DTOs;

public class CreateBundleItemRequest
{
    public int BundleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal EffectiveUnitPrice { get; set; }
}
