namespace StockDaddy.Application.DTOs;

public class CreateUserRequest
{
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public int? StoreId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
