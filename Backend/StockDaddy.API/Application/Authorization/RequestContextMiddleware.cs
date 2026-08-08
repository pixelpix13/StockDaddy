using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using StockDaddy.Infrastructure.Persistence;

using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.Authorization;

/// <summary>
/// Resolves tenant, allowed stores, and active store from JWT claims and optional X-Store-Id header.
/// </summary>
public sealed class RequestContextMiddleware
{
    public const string StoreIdHeader = "X-Store-Id";

    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext db, RequestContextHolder holder)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = ParseIntClaim(httpContext.User, ClaimTypes.NameIdentifier);
            var tenantId = ParseIntClaim(httpContext.User, "tenantId");
            var jwtStoreId = ParseIntClaim(httpContext.User, "storeId");

            var permissions = httpContext.User
                .FindAll(PermissionKeys.ClaimType)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var roleName = httpContext.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var canAccessAll = PermissionKeys.GrantsAccessToAllStores(roleName, permissions);

            IReadOnlyList<int> allowedStoreIds = [];
            int? defaultStoreId = jwtStoreId;

            if (userId.HasValue && tenantId.HasValue)
            {
                if (canAccessAll)
                {
                    allowedStoreIds = await db.Stores
                        .Where(s => !s.IsDeleted && s.TenantId == tenantId.Value)
                        .Select(s => s.Id)
                        .ToListAsync();
                }
                else
                {
                    var assignments = await db.UserStores
                        .Where(us => us.UserId == userId.Value)
                        .Join(db.Stores.Where(s => !s.IsDeleted && s.TenantId == tenantId.Value),
                            us => us.StoreId,
                            s => s.Id,
                            (us, s) => new { us.StoreId, us.IsDefault })
                        .ToListAsync();

                    if (assignments.Count > 0)
                    {
                        allowedStoreIds = assignments.Select(a => a.StoreId).Distinct().ToList();
                        defaultStoreId = assignments.FirstOrDefault(a => a.IsDefault)?.StoreId
                                         ?? assignments.First().StoreId;
                    }
                    else
                    {
                        var legacyStoreId = await db.Users
                            .Where(u => u.Id == userId.Value && !u.IsDeleted)
                            .Select(u => u.StoreId)
                            .FirstOrDefaultAsync();
                        if (legacyStoreId.HasValue)
                        {
                            allowedStoreIds = [legacyStoreId.Value];
                            defaultStoreId = legacyStoreId;
                        }
                    }
                }
            }

            int? activeStoreId = null;
            if (httpContext.Request.Headers.TryGetValue(StoreIdHeader, out var headerValue) &&
                int.TryParse(headerValue.FirstOrDefault(), out var headerStoreId))
            {
                if (canAccessAll || allowedStoreIds.Contains(headerStoreId))
                {
                    activeStoreId = headerStoreId;
                }
            }

            activeStoreId ??= jwtStoreId ?? defaultStoreId;

            if (activeStoreId.HasValue && !canAccessAll && !allowedStoreIds.Contains(activeStoreId.Value))
            {
                activeStoreId = defaultStoreId;
            }

            holder.Context = new RequestContext
            {
                IsAuthenticated = true,
                UserId = userId,
                TenantId = tenantId,
                ActiveStoreId = activeStoreId,
                CanAccessAllTenantStores = canAccessAll,
                AllowedStoreIds = allowedStoreIds
            };
        }
        else
        {
            holder.Context = new RequestContext { IsAuthenticated = false };
        }

        await _next(httpContext);
    }

    private static int? ParseIntClaim(ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirstValue(claimType);
        return int.TryParse(value, out var id) ? id : null;
    }
}

/// <summary>Mutable holder so middleware can set scoped IRequestContext instance.</summary>
public sealed class RequestContextHolder : IRequestContext
{
    public RequestContext Context { get; set; } = new() { IsAuthenticated = false };

    public bool IsAuthenticated => Context.IsAuthenticated;
    public int? UserId => Context.UserId;
    public int? TenantId => Context.TenantId;
    public int? ActiveStoreId => Context.ActiveStoreId;
    public bool CanAccessAllTenantStores => Context.CanAccessAllTenantStores;
    public IReadOnlyList<int> AllowedStoreIds => Context.AllowedStoreIds;
    public bool IsStoreAllowed(int storeId) => Context.IsStoreAllowed(storeId);
}
