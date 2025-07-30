namespace StockDaddy.Application.DTOs;
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
