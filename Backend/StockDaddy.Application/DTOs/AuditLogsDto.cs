namespace StockDaddy.Application.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? StoreId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string OldData { get; set; } = string.Empty;
    public string NewData { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
