namespace StockDaddy.Application.Authorization;

public sealed class RequestContext : IRequestContext
{
    public bool IsAuthenticated { get; init; }
    public int? UserId { get; init; }
    public int? TenantId { get; init; }
    public int? ActiveStoreId { get; init; }
    public bool CanAccessAllTenantStores { get; init; }
    public IReadOnlyList<int> AllowedStoreIds { get; init; } = [];

    public bool IsStoreAllowed(int storeId) =>
        CanAccessAllTenantStores || AllowedStoreIds.Contains(storeId);
}
