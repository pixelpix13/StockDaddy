namespace StockDaddy.Application.DTOs;
public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? AttributeName { get; set; }
    public string? AttributeValue { get; set; }
}
