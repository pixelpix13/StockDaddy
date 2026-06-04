namespace StockDaddy.Application.DTOs;

public class UpdateHsnMasterRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal CGSTPercent { get; set; }
    public decimal SGSTPercent { get; set; }
}
