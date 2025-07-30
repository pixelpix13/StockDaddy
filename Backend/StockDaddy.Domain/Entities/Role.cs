namespace StockDaddy.Domain.Entities;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();  // use GUID instead of int
    public string Name { get; set; } = string.Empty;

    // Optional: Navigation
    public ICollection<User>? Users { get; set; }
}
