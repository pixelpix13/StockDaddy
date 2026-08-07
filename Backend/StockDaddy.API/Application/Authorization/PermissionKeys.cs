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
        "Users",
        "AccessControl",
        "Settings",
        "BillAdjustment",
    ];

    public static string Format(string module, PermissionAction action) =>
        $"{module}:{action}";

    public static string Format(string module, string action) =>
        $"{module}:{action}";

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
