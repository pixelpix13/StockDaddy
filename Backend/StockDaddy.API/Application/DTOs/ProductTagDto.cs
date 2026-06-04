namespace StockDaddy.Application.DTOs;

public class ProductTagDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
