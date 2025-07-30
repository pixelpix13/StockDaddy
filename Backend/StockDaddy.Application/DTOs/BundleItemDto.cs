namespace StockDaddy.Application.DTOs;
public class BundleItemDto
{
    public Guid Id { get; set; }
    public Guid BundleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal EffectiveUnitPrice { get; set; }
}
