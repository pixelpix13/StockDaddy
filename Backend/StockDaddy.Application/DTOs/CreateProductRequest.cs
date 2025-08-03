namespace StockDaddy.Application.DTOs;

public class CreateProductRequest
{
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? SubcategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
}
