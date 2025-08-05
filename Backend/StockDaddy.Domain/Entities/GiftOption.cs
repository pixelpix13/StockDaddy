namespace StockDaddy.Domain.Entities;

public class GiftOption
{
    public int Id { get; set; }
    public int? SaleId { get; set; }

    public bool IsWrapped { get; set; }
    public string? WrapType { get; set; }
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Sale? Sale { get; set; }
}
