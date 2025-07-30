namespace StockDaddy.Application.DTOs;
public class UpdateUserRequest
{
    public Guid RoleId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
}
