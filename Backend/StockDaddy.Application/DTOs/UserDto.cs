namespace StockDaddy.Application.DTOs;
public class UserDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
}
