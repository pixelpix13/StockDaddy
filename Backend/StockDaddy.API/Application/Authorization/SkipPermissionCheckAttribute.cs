namespace StockDaddy.Application.Authorization;

/// <summary>
/// Skips fine-grained permission checks. Endpoint still requires authentication unless AllowAnonymous.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipPermissionCheckAttribute : Attribute;
