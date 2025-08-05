namespace StockDaddy.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
