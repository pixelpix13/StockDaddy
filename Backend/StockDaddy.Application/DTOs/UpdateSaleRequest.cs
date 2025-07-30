namespace StockDaddy.Application.DTOs;

public class UpdateSaleRequest
{
    public Guid? CustomerId { get; set; } // Optional update
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string? Notes { get; set; }
}
