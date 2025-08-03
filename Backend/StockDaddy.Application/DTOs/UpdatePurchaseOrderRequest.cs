using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class UpdatePurchaseOrderRequest
{
    public DateTime ExpectedDelivery { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}
