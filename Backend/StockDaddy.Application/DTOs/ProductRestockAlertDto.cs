namespace StockDaddy.Application.DTOs;

public class ProductRestockAlertDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int VariantId { get; set; }

    public DateTime TriggeredAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
