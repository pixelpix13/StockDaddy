namespace StockDaddy.Application.DTOs;
public class CreateProductTagRequest
{
    public Guid ProductId { get; set; }
    public string Tag { get; set; } = "";
}
