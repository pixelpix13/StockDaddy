using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int SupplierId { get; set; }
    public int StoreId { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDelivery { get; set; }

    public PurchaseOrderStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
