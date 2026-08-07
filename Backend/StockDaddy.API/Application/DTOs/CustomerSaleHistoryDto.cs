namespace StockDaddy.Application.DTOs;

public class SaleItemDetailDto
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CustomerSaleHistoryDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public List<SaleItemDetailDto> Items { get; set; } = new();
}
