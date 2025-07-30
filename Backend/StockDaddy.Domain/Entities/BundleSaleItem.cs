namespace StockDaddy.Domain.Entities;

public class BundleSaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SaleId { get; set; }
    public Guid BundleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public Sale? Sale { get; set; }
    public ProductBundle? Bundle { get; set; }
}
