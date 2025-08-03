namespace StockDaddy.Domain.Entities;

public class IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StoreId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty; // JSON stored as string

    public Guid TriggeredBy { get; set; }
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

    public bool Delivered { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    

    // Navigation
    public Store? Store { get; set; }
    public User? TriggeredByUser { get; set; }
}
