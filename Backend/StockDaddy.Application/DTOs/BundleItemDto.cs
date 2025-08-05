namespace StockDaddy.Application.DTOs;

public class BundleItemDto
{
    public int Id { get; set; }
    public int BundleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal EffectiveUnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
