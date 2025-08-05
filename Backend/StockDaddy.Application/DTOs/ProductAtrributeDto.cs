namespace StockDaddy.Application.DTOs;

public class ProductAttributeDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
