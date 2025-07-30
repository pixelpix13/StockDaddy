namespace StockDaddy.Application.DTOs;
public class CreateProductVariantRequest
{
    public Guid ProductId { get; set; }
    public string VariantName { get; set; } = "";
    public string SkuCode { get; set; } = "";
    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public int Quantity { get; set; }
}
