namespace StockDaddy.Application.DTOs;

public class CreateAuditLogRequest
{
    public int UserId { get; set; }
    public int? StoreId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string OldData { get; set; } = string.Empty;
    public string NewData { get; set; } = string.Empty;
}
