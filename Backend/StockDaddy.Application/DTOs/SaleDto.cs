using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class SaleDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
