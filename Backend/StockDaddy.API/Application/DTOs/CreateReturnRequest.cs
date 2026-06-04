namespace StockDaddy.Application.DTOs;

public class CreateReturnRequest
{
    public int SaleId { get; set; }
    public int? StoreId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
