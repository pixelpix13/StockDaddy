using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Module { get; set; } = string.Empty;
    public PermissionAction Action { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Optional: Navigation
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
