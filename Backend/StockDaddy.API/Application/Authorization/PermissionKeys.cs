using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.Authorization;

/// <summary>
/// Canonical permission modules and helpers. Permission key format: "{Module}:{Action}".
/// </summary>
public static class PermissionKeys
{
    public const string ClaimType = "permission";

    public static readonly string[] Modules =
    [
        "Dashboard",
        "Catalog",
        "Product",
        "Inventory",
        "Sales",
        "Purchase",
        "Supplier",
        "Customer",
        "Company",
        "Users",
        "AccessControl",
        "Settings",
        "BillAdjustment",
        "Activity",
        "Credit",
    ];

    public static string Format(string module, PermissionAction action) =>
        $"{module}:{action}";

    public static string Format(string module, string action) =>
        $"{module}:{action}";

    /// <summary>Admin (owner) or explicit Settings:AccessAllStores may use any store in the tenant.</summary>
    public static bool GrantsAccessToAllStores(string? roleName, IEnumerable<string> permissions) =>
        (!string.IsNullOrEmpty(roleName) &&
         roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ||
        permissions.Contains(Format("Settings", PermissionAction.AccessAllStores), StringComparer.OrdinalIgnoreCase);

    public static bool TryParse(string key, out string module, out PermissionAction action)
    {
        module = string.Empty;
        action = default;

        var parts = key.Split(':', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        module = parts[0];
        return Enum.TryParse(parts[1], ignoreCase: true, out action);
    }
}
