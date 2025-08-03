namespace StockDaddy.Application.DTOs;

public class CreateUserRequest
{
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? StoreId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
