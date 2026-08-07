namespace StockDaddy.Application.DTOs;

public class PermissionSummaryDto
{
    public int Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}

public class RoleWithPermissionsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = [];
}

public class UpdateRolePermissionsRequest
{
    public List<int> PermissionIds { get; set; } = [];
}

public class AssignUserRoleRequest
{
    public int RoleId { get; set; }
}

public class RbacMatrixDto
{
    public List<PermissionSummaryDto> Permissions { get; set; } = [];
    public List<RoleWithPermissionsDto> Roles { get; set; } = [];
}
