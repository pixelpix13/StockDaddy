namespace StockDaddy.Application.DTOs;

public class UpdateGiftOptionRequest
{
    public bool IsWrapped { get; set; }
    public string? WrapType { get; set; }
    public string? Message { get; set; }
}
