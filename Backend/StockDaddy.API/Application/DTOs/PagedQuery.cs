namespace StockDaddy.Application.DTOs;

/// <summary>
/// Standard list query: pagination, sort, and search.
/// </summary>
public class PagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDir { get; set; } = "asc";
    public string? Search { get; set; }

    /// <summary>Optional filters — applied when set by list endpoints that support them.</summary>
    public int? CategoryId { get; set; }
    public string? Status { get; set; }
    public int? RoleId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? StockFilter { get; set; }
    public int? SupplierId { get; set; }
    public int? SubcategoryId { get; set; }
    public decimal? TaxPercent { get; set; }
    /// <summary>Filter audit logs by entity/controller name (TableName).</summary>
    public string? Entity { get; set; }
    /// <summary>Filter audit logs by user who performed the action.</summary>
    public int? UserId { get; set; }
    /// <summary>Filter sales by customer.</summary>
    public int? CustomerId { get; set; }
    /// <summary>Filter credit ledger by party type (Customer/Supplier).</summary>
    public string? PartyType { get; set; }
}
