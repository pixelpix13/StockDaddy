using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class UpdateSaleRequest
{
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}
