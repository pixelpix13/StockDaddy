namespace StockDaddy.Application.DTOs;

public class CreateCategoryRequest
{
    public int StoreId { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
}
