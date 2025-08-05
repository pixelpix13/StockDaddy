namespace StockDaddy.Application.DTOs;
public class CreateSaleItemRequest
{
    public int SaleId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
