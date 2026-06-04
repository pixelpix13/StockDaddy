using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreatePurchaseOrderRequest
{
    public int TenantId { get; set; }
    public int SupplierId { get; set; }
    public int StoreId { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDelivery { get; set; }

    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
    public string Notes { get; set; } = string.Empty;
}
