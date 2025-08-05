namespace StockDaddy.Application.DTOs;
using StockDaddy.Domain.Enums;
public class StockItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? StoreId { get; set; }
    public int Quantity { get; set; }

    public StockStatus Status { get; set; }
    public DateTime LastUpdated { get; set; }
    public int? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}