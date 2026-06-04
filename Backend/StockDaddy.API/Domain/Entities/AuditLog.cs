namespace StockDaddy.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public int? StoreId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string OldData { get; set; } = string.Empty;
    public string NewData { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    

    // Navigation
    public User? User { get; set; }
    public Store? Store { get; set; }
}
