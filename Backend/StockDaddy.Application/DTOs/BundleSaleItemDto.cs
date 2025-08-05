namespace StockDaddy.Application.DTOs;

public class BundleSaleItemDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int BundleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
