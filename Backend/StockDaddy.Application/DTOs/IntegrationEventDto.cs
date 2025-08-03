namespace StockDaddy.Application.DTOs;

public class IntegrationEventDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public Guid TriggeredBy { get; set; }
    public DateTime TriggeredAt { get; set; }
    public bool Delivered { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
