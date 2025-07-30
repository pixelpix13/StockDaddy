namespace StockDaddy.Application.DTOs;
public class CreateProductAttributeRequest
{
    public Guid ProductId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
}
