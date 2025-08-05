using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreateShipmentRequest
{
    public int SaleId { get; set; }
    public int StoreId { get; set; }

    public string CourierName { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;

    public DateTime ShippedDate { get; set; }
    public DateTime EstimatedArrival { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
}
