namespace StockDaddy.Domain.Entities;

/// <summary>
/// Many-to-many: which stores a user may access within their tenant.
/// </summary>
public class UserStore
{
    public int UserId { get; set; }
    public int StoreId { get; set; }
    public int RoleId { get; set; }
    public bool IsDefault { get; set; }

    public User? User { get; set; }
    public Store? Store { get; set; }
    public Role? Role { get; set; }
}
