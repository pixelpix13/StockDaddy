namespace StockDaddy.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = "";
    public bool IsPrimary { get; set; }

    // Optional navigation
    public Product? Product { get; set; }
}
