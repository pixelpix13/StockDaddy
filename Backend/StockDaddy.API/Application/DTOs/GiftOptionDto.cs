namespace StockDaddy.Application.DTOs;

public class GiftOptionDto
{
    public int Id { get; set; }
    public int? SaleId { get; set; }
    public bool IsWrapped { get; set; }
    public string? WrapType { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
