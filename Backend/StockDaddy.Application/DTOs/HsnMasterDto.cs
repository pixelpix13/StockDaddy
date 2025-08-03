namespace StockDaddy.Application.DTOs;

public class HsnMasterDto
{
    public Guid Id { get; set; }
    public string HSNCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CGSTPercent { get; set; }
    public decimal SGSTPercent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
