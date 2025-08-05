using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreateSaleRequest
{
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? CustomerId { get; set; }
    public int SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}
