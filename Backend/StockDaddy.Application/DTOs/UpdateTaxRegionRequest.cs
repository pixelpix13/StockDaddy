namespace StockDaddy.Application.DTOs;

public class UpdateTaxRegionRequest
{
    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }
}
