namespace StockDaddy.Application.DTOs;

public class ProductTagDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
