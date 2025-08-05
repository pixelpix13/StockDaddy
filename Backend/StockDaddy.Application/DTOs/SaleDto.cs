using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? CustomerId { get; set; }
    public int SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
