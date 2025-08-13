namespace StockDaddy.Application.DTOs;

public class CreateProductRequest
{
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? SubcategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
}
