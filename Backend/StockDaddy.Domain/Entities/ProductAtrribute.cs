namespace StockDaddy.Domain.Entities;

public class ProductAttribute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;

    public Product? Product { get; set; }
}
