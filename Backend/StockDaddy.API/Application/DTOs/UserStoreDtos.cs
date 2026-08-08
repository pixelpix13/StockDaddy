namespace StockDaddy.Application.DTOs;

public class UserStoreAssignmentDto
{
    public int StoreId { get; set; }
    public int RoleId { get; set; }
    public bool IsDefault { get; set; }
    public string? StoreName { get; set; }
    public string? RoleName { get; set; }
}

public class UserStoreOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class SwitchStoreRequest
{
    public int StoreId { get; set; }
}

public class AssignUserStoreAssignmentsRequest
{
    /// <summary>Fallback role when no store-specific assignment exists.</summary>
    public int? DefaultRoleId { get; set; }
    public int? DefaultStoreId { get; set; }
    public List<UserStoreAssignmentDto> Assignments { get; set; } = [];
}
