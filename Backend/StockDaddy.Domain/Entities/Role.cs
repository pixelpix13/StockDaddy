namespace StockDaddy.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;    

    // Navigation
    public ICollection<User>? Users { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
