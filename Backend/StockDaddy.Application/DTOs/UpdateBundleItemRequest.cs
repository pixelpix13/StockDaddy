namespace StockDaddy.Application.DTOs;

public class UpdateBundleItemRequest
{
    public int? Quantity { get; set; }
    public decimal? EffectiveUnitPrice { get; set; }
}
