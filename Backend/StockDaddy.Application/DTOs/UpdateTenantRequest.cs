namespace StockDaddy.Application.DTOs;

public class UpdateTenantRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

