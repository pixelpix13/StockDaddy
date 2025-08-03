namespace StockDaddy.Application.DTOs;

public class SubcategoryDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
