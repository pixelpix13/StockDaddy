namespace StockDaddy.Application.DTOs;

public class TaxRegionDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public decimal TaxPercent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
