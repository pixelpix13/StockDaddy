namespace StockDaddy.Application.DTOs;
public class CreateSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
