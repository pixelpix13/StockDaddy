namespace StockDaddy.Domain.Entities;

public class ProductVariant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }  // FK
    public string VariantName { get; set; } = "";
    public string SkuCode { get; set; } = "";
    
    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }

    public decimal Price { get; set; }  // Final selling price
    public int Quantity { get; set; }

    public Product? Product { get; set; }  // Navigation property
}
