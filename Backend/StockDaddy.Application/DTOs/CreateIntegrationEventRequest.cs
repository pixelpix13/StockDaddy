namespace StockDaddy.Application.DTOs;

public class CreateIntegrationEventRequest
{
    public int StoreId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public int TriggeredBy { get; set; }
}
