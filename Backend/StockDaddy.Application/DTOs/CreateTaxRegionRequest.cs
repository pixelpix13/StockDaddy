namespace StockDaddy.Application.DTOs;

public class CreateTaxRegionRequest
{
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }
}
