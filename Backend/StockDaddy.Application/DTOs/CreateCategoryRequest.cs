namespace StockDaddy.Application.DTOs;

public class CreateCategoryRequest
{
    public Guid StoreId { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
}
