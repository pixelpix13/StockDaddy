namespace StockDaddy.Application.DTOs;

public class CreateSubcategoryRequest
{
    public int StoreId { get; set; }
    public int TenantId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}
