namespace StockDaddy.Application.DTOs;

public class CreateCustomerRequest
{
    public int TenantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
