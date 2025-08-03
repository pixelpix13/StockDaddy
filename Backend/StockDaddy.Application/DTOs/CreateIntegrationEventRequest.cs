namespace StockDaddy.Application.DTOs;

public class CreateIntegrationEventRequest
{
    public Guid StoreId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public Guid TriggeredBy { get; set; }
}
