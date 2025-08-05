namespace StockDaddy.Application.DTOs;  
public class SaleItemDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
