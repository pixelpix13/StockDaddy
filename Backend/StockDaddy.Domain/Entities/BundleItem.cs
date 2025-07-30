namespace StockDaddy.Domain.Entities;

public class BundleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BundleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal EffectiveUnitPrice { get; set; }

    public ProductBundle? Bundle { get; set; }
    public Product? Product { get; set; }
}
