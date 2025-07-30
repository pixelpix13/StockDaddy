namespace StockDaddy.Domain.Entities;

public class Subcategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Category? Category { get; set; }
}
