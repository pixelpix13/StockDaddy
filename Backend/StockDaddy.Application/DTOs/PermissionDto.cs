using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public PermissionAction Action { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
