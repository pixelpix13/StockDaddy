namespace StockDaddy.Application.DTOs;

public class UpdateRolePermissionRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
