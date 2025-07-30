namespace StockDaddy.Application.DTOs;

public class SubcategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid TenantId { get; set; }  
}
