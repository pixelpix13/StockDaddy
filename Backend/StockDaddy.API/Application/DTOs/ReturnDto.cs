namespace StockDaddy.Application.DTOs;

public class ReturnDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int? StoreId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
