namespace StockDaddy.Application.DTOs;

public class CreateTaxRegionRequest
{
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }
}
