namespace StockDaddy.Application.DTOs;

public class CreateProductRestockAlertRequest
{
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int VariantId { get; set; }
    public string Status { get; set; } = "Pending";
}
