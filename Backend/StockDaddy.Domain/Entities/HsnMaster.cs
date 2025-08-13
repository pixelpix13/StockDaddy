namespace StockDaddy.Domain.Entities;

public class HsnMaster
{
    public int Id { get; set; }
    public string HSNCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CGSTPercent { get; set; }
    public decimal SGSTPercent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public ICollection<ProductVariant>? ProductVariants { get; set; }
}
