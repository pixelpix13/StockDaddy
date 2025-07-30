namespace StockDaddy.Application.DTOs;

public class CreateGiftOptionRequest

{
    public Guid SaleId { get; set; }
    public bool IsWrapped { get; set; }
    public string WrapType { get; set; } = "";
    public string? Message { get; set; }
}
