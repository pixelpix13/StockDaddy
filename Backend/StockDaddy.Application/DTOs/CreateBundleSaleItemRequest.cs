namespace StockDaddy.Application.DTOs;

public class CreateBundleSaleItemRequest
{
    public int SaleId { get; set; }
    public int BundleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
}
