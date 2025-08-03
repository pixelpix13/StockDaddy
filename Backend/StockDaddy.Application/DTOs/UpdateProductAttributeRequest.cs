namespace StockDaddy.Application.DTOs;

public class UpdateProductAttributeRequest
{
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
}
