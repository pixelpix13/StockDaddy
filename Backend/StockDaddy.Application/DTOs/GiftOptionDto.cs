namespace StockDaddy.Application.DTOs;

public class GiftOptionDto
{
    public Guid Id { get; set; }
    public Guid? SaleId { get; set; }
    public bool IsWrapped { get; set; }
    public string? WrapType { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
