namespace StockDaddy.Application.DTOs;

public class TaxRegionDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
