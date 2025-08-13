using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Shipment
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public int StoreId { get; set; }

    public string CourierName { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;

    public DateTime ShippedDate { get; set; }
    public DateTime EstimatedArrival { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation (optional)
    public Sale? Sale { get; set; }
    public Store? Store { get; set; }
}
