namespace StockDaddy.Application.DTOs;
public class CreateCategoryRequest
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
}
