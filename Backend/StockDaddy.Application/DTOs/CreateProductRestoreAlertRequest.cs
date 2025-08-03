namespace StockDaddy.Application.DTOs;

public class CreateProductRestockAlertRequest
{
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public Guid VariantId { get; set; }
    public string Status { get; set; } = "Pending";
}
