namespace StockDaddy.Application.DTOs;

public class CreateStoreRequest
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
