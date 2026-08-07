using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StockDaddy.Application.Authorization;

/// <summary>
/// Enforces fine-grained permission claims on every authenticated endpoint.
/// </summary>
public sealed class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            return Task.CompletedTask;
        }

        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipPermissionCheckAttribute>().Any())
        {
            return Task.CompletedTask;
        }

        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        var requiredPermission = EndpointPermissionResolver.Resolve(context);
        if (string.IsNullOrEmpty(requiredPermission))
        {
            return Task.CompletedTask;
        }

        var granted = user.FindAll(PermissionKeys.ClaimType).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (granted.Contains(requiredPermission))
        {
            return Task.CompletedTask;
        }

        context.Result = new ObjectResult(new
        {
            message = $"Forbidden. Required permission: {requiredPermission}"
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        return Task.CompletedTask;
    }
}
