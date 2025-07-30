namespace StockDaddy.Application.DTOs;
public class UpdateProductVariantRequest
{
    public string VariantName { get; set; } = "";
    public string SkuCode { get; set; } = "";
    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public int Quantity { get; set; }
}
