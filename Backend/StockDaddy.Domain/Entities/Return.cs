namespace StockDaddy.Domain.Entities;

public class Return
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SaleId { get; set; }
    public Guid? StoreId { get; set; }
    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;


    // Navigation
    public Sale? Sale { get; set; }
    public Store? Store { get; set; }
}
