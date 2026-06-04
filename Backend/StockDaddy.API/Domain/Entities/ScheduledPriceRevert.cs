namespace StockDaddy.Domain.Entities;

public class ScheduledPriceRevert
{
    public int Id { get; set; }

    /// <summary>
    /// "category" or "product"
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// CategoryId or ProductId depending on Type.
    /// Not a strict navigation—use only for lookups.
    /// </summary>
    public int RefId { get; set; }

    /// <summary>
    /// JSON: ProductVariantId:price map for reverting prices.
    /// </summary>
    public string OriginalPricesJson { get; set; } = string.Empty;
    public string? BatchCriteria { get; set; }


    public DateTime RevertAt { get; set; }
    public bool IsCompleted { get; set; } = false;

    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ================== Navigation ===================
    /// <summary>
    /// Navigation to the user who scheduled the revert
    /// </summary>
    public User? CreatedByUser { get; set; }
}
