namespace StockDaddy.Application.DTOs;
using StockDaddy.Domain.Enums;

public class CreateStockItemRequest
{
    public int ProductId { get; set; }
    public int? StoreId { get; set; }
    public int Quantity { get; set; }
    public StockStatus Status { get; set; } = StockStatus.InStock;
    public int? UpdatedBy { get; set; }
}
