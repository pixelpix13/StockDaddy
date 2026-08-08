using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.Application.Authorization;

/// <summary>
/// Persists an audit log entry after successful mutating API calls (POST/PUT/PATCH/DELETE).
/// </summary>
public sealed class ActivityAuditFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> MutatingMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST", "PUT", "PATCH", "DELETE"
    };

    private static readonly HashSet<string> IgnoredControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Auth"
    };

    private readonly IAuditLogRepository _auditLogRepository;
    private readonly RequestContextHolder _requestContext;
    private readonly ILogger<ActivityAuditFilter> _logger;

    public ActivityAuditFilter(
        IAuditLogRepository auditLogRepository,
        RequestContextHolder requestContext,
        ILogger<ActivityAuditFilter> logger)
    {
        _auditLogRepository = auditLogRepository;
        _requestContext = requestContext;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        try
        {
            await TryLogActivityAsync(context, executedContext);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write activity audit log");
        }
    }

    private async Task TryLogActivityAsync(ActionExecutingContext executing, ActionExecutedContext executed)
    {
        if (executed.Exception != null)
        {
            return;
        }

        var http = executing.HttpContext;
        if (!MutatingMethods.Contains(http.Request.Method))
        {
            return;
        }

        var status = http.Response.StatusCode;
        if (status < 200 || status >= 300)
        {
            return;
        }

        if (executing.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        if (IgnoredControllers.Contains(descriptor.ControllerName))
        {
            return;
        }

        if (string.Equals(descriptor.ControllerName, "AuditLog", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        int? storeId = _requestContext.ActiveStoreId;
        if (!storeId.HasValue)
        {
            var storeClaim = http.User.FindFirst("storeId")?.Value;
            if (int.TryParse(storeClaim, out var parsedStoreId))
            {
                storeId = parsedStoreId;
            }
        }

        var auditAction = MapHttpMethod(http.Request.Method);
        var recordId = ExtractRecordId(executing, executed);
        var payload = BuildPayloadSummary(executing, descriptor);

        await _auditLogRepository.AddAsync(new CreateAuditLogRequest
        {
            UserId = userId,
            StoreId = storeId,
            Action = auditAction,
            TableName = descriptor.ControllerName,
            RecordId = recordId,
            OldData = string.Empty,
            NewData = $"{descriptor.ActionName}: {payload}"
        });
    }

    private static string MapHttpMethod(string method) =>
        method.ToUpperInvariant() switch
        {
            "POST" => "Create",
            "PUT" or "PATCH" => "Update",
            "DELETE" => "Delete",
            _ => method
        };

    private static string ExtractRecordId(ActionExecutingContext executing, ActionExecutedContext executed)
    {
        if (executing.RouteData.Values.TryGetValue("id", out var routeId) && routeId != null)
        {
            return routeId.ToString() ?? "n/a";
        }

        if (executed.Result is CreatedAtActionResult createdAt &&
            createdAt.RouteValues != null &&
            createdAt.RouteValues.TryGetValue("id", out var createdId) &&
            createdId != null)
        {
            return createdId.ToString() ?? "n/a";
        }

        if (executed.Result is ObjectResult { Value: not null } objectResult)
        {
            var idProperty = objectResult.Value.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                return idProperty.GetValue(objectResult.Value)?.ToString() ?? "n/a";
            }
        }

        foreach (var argument in executing.ActionArguments)
        {
            if (argument.Key.Equals("id", StringComparison.OrdinalIgnoreCase) && argument.Value != null)
            {
                return argument.Value.ToString() ?? "n/a";
            }
        }

        return "n/a";
    }

    private static string BuildPayloadSummary(ActionExecutingContext executing, ControllerActionDescriptor descriptor)
    {
        var payload = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["method"] = executing.HttpContext.Request.Method,
            ["path"] = executing.HttpContext.Request.Path.Value
        };

        foreach (var argument in executing.ActionArguments)
        {
            if (argument.Value is CancellationToken)
            {
                continue;
            }

            if (argument.Key.Equals("query", StringComparison.OrdinalIgnoreCase) &&
                argument.Value is PagedQuery)
            {
                continue;
            }

            payload[argument.Key] = argument.Value;
        }

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        return json.Length <= 4000 ? json : json[..4000];
    }
}
