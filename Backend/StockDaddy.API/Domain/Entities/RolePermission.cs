namespace StockDaddy.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
