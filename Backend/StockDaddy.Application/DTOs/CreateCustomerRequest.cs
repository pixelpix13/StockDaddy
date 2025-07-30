namespace StockDaddy.Application.DTOs;
public class CreateCustomerRequest
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
}
