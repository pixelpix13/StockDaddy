namespace StockDaddy.Application.DTOs;
using StockDaddy.Domain.Enums;
public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? StoreId { get; set; }
    public int Quantity { get; set; }

    public StockStatus Status { get; set; }
    public DateTime LastUpdated { get; set; }
    public Guid? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}