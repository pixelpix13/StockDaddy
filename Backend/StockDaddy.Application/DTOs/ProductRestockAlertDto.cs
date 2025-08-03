namespace StockDaddy.Application.DTOs;

public class ProductRestockAlertDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public Guid VariantId { get; set; }

    public DateTime TriggeredAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
