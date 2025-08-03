namespace StockDaddy.Application.DTOs;

public class BundleSaleItemDto
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid BundleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
