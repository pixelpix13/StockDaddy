namespace StockDaddy.Application.DTOs;

public class CreateHsnMasterRequest
{
    public string HSNCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CGSTPercent { get; set; }
    public decimal SGSTPercent { get; set; }
}
