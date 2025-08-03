using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreateSaleRequest
{
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}
