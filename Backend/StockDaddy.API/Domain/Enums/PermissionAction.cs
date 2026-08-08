namespace StockDaddy.Domain.Enums;

public enum PermissionAction
{
    Read,
    Write,
    Update,
    Delete,
    /// <summary>Switch to and operate across all tenant stores (Settings module only).</summary>
    AccessAllStores,
}
