namespace StockDaddy.Application.DTOs;

public class CreateSubcategoryRequest
{
    public Guid StoreId { get; set; }
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}
