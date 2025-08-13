namespace StockDaddy.Application.DTOs;

public class SubcategoryDto
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int TenantId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
