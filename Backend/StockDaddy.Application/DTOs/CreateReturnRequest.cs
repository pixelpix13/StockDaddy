namespace StockDaddy.Application.DTOs;

public class CreateReturnRequest
{
    public Guid SaleId { get; set; }
    public Guid? StoreId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
