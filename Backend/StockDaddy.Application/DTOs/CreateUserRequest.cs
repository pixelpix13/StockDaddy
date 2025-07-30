namespace StockDaddy.Application.DTOs;
public class CreateUserRequest
{
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = ""; // plain password to be hashed
}
