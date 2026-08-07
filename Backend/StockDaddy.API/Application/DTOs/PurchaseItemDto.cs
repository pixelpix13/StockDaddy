namespace StockDaddy.Application.DTOs;

public class PurchaseItemDto
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public int? QuantityReceived { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }

    public string? ProductName { get; set; }
    public string? SkuCode { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
