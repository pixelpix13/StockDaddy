namespace StockDaddy.Application.DTOs;

public class UpdateUserRequest
{
    public int RoleId { get; set; }
    public int? StoreId { get; set; }
    public List<int> StoreIds { get; set; } = [];
    public int? DefaultStoreId { get; set; }
    public List<UserStoreAssignmentDto> StoreAssignments { get; set; } = [];

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
