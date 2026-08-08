using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Helpers;

public static class QueryScope
{
    public static int? ResolveStoreFilter(PagedQuery query, IRequestContext ctx)
    {
        var requested = query.StoreId ?? ctx.ActiveStoreId;
        if (!requested.HasValue)
        {
            return null;
        }

        if (ctx.CanAccessAllTenantStores || ctx.IsStoreAllowed(requested.Value))
        {
            return requested;
        }

        return ctx.ActiveStoreId;
    }

    public static IQueryable<T> ApplyTenantFilter<T>(
        IQueryable<T> query,
        IRequestContext ctx,
        System.Linq.Expressions.Expression<Func<T, int>> tenantSelector)
    {
        if (!ctx.TenantId.HasValue)
        {
            return query;
        }

        var tenantId = ctx.TenantId.Value;
        var parameter = tenantSelector.Parameters[0];
        var body = System.Linq.Expressions.Expression.Equal(
            tenantSelector.Body,
            System.Linq.Expressions.Expression.Constant(tenantId));
        var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, parameter);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplyStoreFilter<T>(
        IQueryable<T> query,
        PagedQuery pagedQuery,
        IRequestContext ctx,
        System.Linq.Expressions.Expression<Func<T, int>> storeSelector)
    {
        var storeId = ResolveStoreFilter(pagedQuery, ctx);
        if (!storeId.HasValue)
        {
            return query;
        }

        var parameter = storeSelector.Parameters[0];
        var body = System.Linq.Expressions.Expression.Equal(
            storeSelector.Body,
            System.Linq.Expressions.Expression.Constant(storeId.Value));
        var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, parameter);
        return query.Where(lambda);
    }
}
