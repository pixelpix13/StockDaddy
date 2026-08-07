using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.Authorization;

/// <summary>
/// Maps controller actions to permission keys for automatic authorization.
/// </summary>
public static class EndpointPermissionResolver
{
    private static readonly Dictionary<string, string> ControllerModuleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Category"] = "Catalog",
        ["Subcategory"] = "Catalog",
        ["HsnMaster"] = "Catalog",
        ["TaxRegion"] = "Catalog",
        ["ProductVariant"] = "Product",
        ["ProductImage"] = "Product",
        ["ProductTag"] = "Product",
        ["ProductAttribute"] = "Product",
        ["ProductBundle"] = "Product",
        ["BundleItem"] = "Product",
        ["BundleSaleItem"] = "Sales",
        ["Sale"] = "Sales",
        ["StockItem"] = "Inventory",
        ["ProductRestockAlert"] = "Inventory",
        ["SaleItem"] = "Sales",
        ["Invoice"] = "Sales",
        ["Payment"] = "Sales",
        ["Refund"] = "Sales",
        ["Return"] = "Sales",
        ["PurchaseItem"] = "Purchase",
        ["Shipment"] = "Purchase",
        ["PurchaseOrder"] = "Purchase",
        ["User"] = "Users",
        ["Role"] = "AccessControl",
        ["Permission"] = "AccessControl",
        ["RolePermission"] = "AccessControl",
        ["Rbac"] = "AccessControl",
        ["Tenant"] = "Settings",
        ["Store"] = "Settings",
        ["BillAdjustment"] = "BillAdjustment",
        ["AdjustedInvoice"] = "BillAdjustment",
        ["ScheduledPriceRevert"] = "Product",
        ["GiftOption"] = "Product",
        ["AuditLog"] = "Activity",
        ["CreditLedger"] = "Credit",
        ["IntegrationEvent"] = "Settings",
    };

    public static string? Resolve(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
        {
            return null;
        }

        var controllerName = actionDescriptor.ControllerName;
        var actionName = actionDescriptor.ActionName;
        var httpMethod = context.HttpContext.Request.Method;

        if (string.Equals(controllerName, "Orchestration", StringComparison.OrdinalIgnoreCase))
        {
            return ResolveOrchestration(actionName, httpMethod);
        }

        if (string.Equals(controllerName, "Auth", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var module = ControllerModuleMap.GetValueOrDefault(controllerName, controllerName);
        if (!PermissionKeys.Modules.Contains(module, StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        var permissionAction = MapHttpMethod(httpMethod);

        if (permissionAction == null)
        {
            return null;
        }

        return PermissionKeys.Format(module, permissionAction.Value);
    }

    private static string? ResolveOrchestration(string actionName, string httpMethod)
    {
        return actionName switch
        {
            nameof(CreateProductWithVariant) => PermissionKeys.Format("Product", PermissionAction.Write),
            nameof(GetVariantByBarcode) => PermissionKeys.Format("Product", PermissionAction.Read),
            nameof(Checkout) => PermissionKeys.Format("Sales", PermissionAction.Write),
            nameof(AdjustStock) => PermissionKeys.Format("Inventory", PermissionAction.Update),
            nameof(GetVariantStock) => PermissionKeys.Format("Inventory", PermissionAction.Read),
            nameof(CreatePurchaseOrderWithItems) => PermissionKeys.Format("Purchase", PermissionAction.Write),
            nameof(GetPurchaseOrderWithItems) => PermissionKeys.Format("Purchase", PermissionAction.Read),
            nameof(UpdatePurchaseOrderWithItems) => PermissionKeys.Format("Purchase", PermissionAction.Update),
            nameof(ReceivePurchaseOrder) => PermissionKeys.Format("Purchase", PermissionAction.Update),
            _ => MapHttpMethod(httpMethod) is { } action
                ? PermissionKeys.Format("Product", action)
                : null
        };
    }

    private static PermissionAction? MapHttpMethod(string httpMethod) =>
        httpMethod.ToUpperInvariant() switch
        {
            "GET" or "HEAD" => PermissionAction.Read,
            "POST" => PermissionAction.Write,
            "PUT" or "PATCH" => PermissionAction.Update,
            "DELETE" => PermissionAction.Delete,
            _ => null
        };

    // Referenced by nameof in orchestration resolver
    private const string CreateProductWithVariant = "CreateProductWithVariant";
    private const string GetVariantByBarcode = "GetVariantByBarcode";
    private const string Checkout = "Checkout";
    private const string AdjustStock = "AdjustStock";
    private const string GetVariantStock = "GetVariantStock";
    private const string CreatePurchaseOrderWithItems = "CreatePurchaseOrderWithItems";
    private const string GetPurchaseOrderWithItems = "GetPurchaseOrderWithItems";
    private const string UpdatePurchaseOrderWithItems = "UpdatePurchaseOrderWithItems";
    private const string ReceivePurchaseOrder = "ReceivePurchaseOrder";
}
