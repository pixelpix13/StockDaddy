namespace StockDaddy.Application.DTOs;

public class UpdateProductVariantRequest
{
    public string VariantName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public int HSNCodeId { get; set; }
}
