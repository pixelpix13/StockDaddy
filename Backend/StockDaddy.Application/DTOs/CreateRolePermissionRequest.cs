namespace StockDaddy.Application.DTOs;

public class CreateRolePermissionRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
