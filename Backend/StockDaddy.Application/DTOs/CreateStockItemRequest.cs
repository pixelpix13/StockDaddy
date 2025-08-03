namespace StockDaddy.Application.DTOs;
using StockDaddy.Domain.Enums;

public class CreateStockItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? StoreId { get; set; }
    public int Quantity { get; set; }
    public StockStatus Status { get; set; } = StockStatus.InStock;
    public Guid? UpdatedBy { get; set; }
}
