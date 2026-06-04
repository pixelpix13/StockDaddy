namespace StockDaddy.Application.DTOs;

public class UpdateSubcategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}
