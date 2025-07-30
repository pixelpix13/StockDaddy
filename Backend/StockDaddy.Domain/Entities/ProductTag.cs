namespace StockDaddy.Domain.Entities;

public class ProductTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }

    public string Tag { get; set; } = "";

    // Optional navigation
    public Product? Product { get; set; }
}
