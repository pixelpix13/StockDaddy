namespace StockDaddy.Application.DTOs;
public class UpdateStockItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid UpdatedBy { get; set; }
}
