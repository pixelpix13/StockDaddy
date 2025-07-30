namespace StockDaddy.Application.DTOs;
public class ProductVariantDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string VariantName { get; set; } = "";
    public string SkuCode { get; set; } = "";
    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
