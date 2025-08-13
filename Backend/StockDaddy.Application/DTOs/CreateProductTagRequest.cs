namespace StockDaddy.Application.DTOs;

public class CreateProductTagRequest
{
    public int ProductId { get; set; }
    public string Tag { get; set; } = string.Empty;
}
