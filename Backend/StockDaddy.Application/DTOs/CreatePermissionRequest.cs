using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreatePermissionRequest
{
    public string Module { get; set; } = string.Empty;
    public PermissionAction Action { get; set; }
}
