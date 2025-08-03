namespace StockDaddy.Application.DTOs;
using StockDaddy.Domain.Enums;

public class UpdateStockItemRequest
{
    public int Quantity { get; set; }
    public StockStatus Status { get; set; } = StockStatus.InStock;
    public Guid? UpdatedBy { get; set; }
}
