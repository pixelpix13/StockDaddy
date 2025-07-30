namespace StockDaddy.Application.DTOs;
public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Status { get; set; } = "in-stock";
    public Guid UpdatedBy { get; set; }
}
