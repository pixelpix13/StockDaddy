namespace StockDaddy.Domain.Entities;

public class GiftOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? SaleId { get; set; }

    public bool IsWrapped { get; set; }
    public string? WrapType { get; set; }
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Sale? Sale { get; set; }
}
