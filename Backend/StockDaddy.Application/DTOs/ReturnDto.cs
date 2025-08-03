namespace StockDaddy.Application.DTOs;

public class ReturnDto
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid? StoreId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
