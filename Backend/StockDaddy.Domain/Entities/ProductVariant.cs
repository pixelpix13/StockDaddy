namespace StockDaddy.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int HSNCodeId { get; set; } // FK to HSNMaster

    public string VariantName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty; // optional but unique
    public string SkuCode { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Product? Product { get; set; }
    public Store? Store { get; set; }
    public HsnMaster? HSNMaster { get; set; }
}
