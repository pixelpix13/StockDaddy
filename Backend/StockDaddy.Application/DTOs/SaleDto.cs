namespace StockDaddy.Application.DTOs;
public class SaleDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public Guid SoldBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
}
