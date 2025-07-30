namespace StockDaddy.Application.DTOs;

public class CreateBundleSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid BundleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
