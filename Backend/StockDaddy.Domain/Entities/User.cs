namespace StockDaddy.Domain.Entities;

public class User
{
    public int Id { get; set; } // Auto-incremented by DB
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public int? StoreId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public Role? Role { get; set; }
    public Store? Store { get; set; }
}
