using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? CustomerId { get; set; }
    public int? CompanyId { get; set; }
    public int SoldBy { get; set; }
    public string? SoldByName { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
