namespace StockDaddy.Application.Authorization;

/// <summary>
/// Per-request tenant and active store context derived from JWT + X-Store-Id header.
/// </summary>
public interface IRequestContext
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    int? TenantId { get; }
    int? ActiveStoreId { get; }
    bool CanAccessAllTenantStores { get; }
    IReadOnlyList<int> AllowedStoreIds { get; }
    bool IsStoreAllowed(int storeId);
}
