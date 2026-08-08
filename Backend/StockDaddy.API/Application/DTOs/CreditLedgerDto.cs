using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreditLedgerDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int StoreId { get; set; }
    public CreditPartyType PartyType { get; set; }
    public CreditStatus Status { get; set; }
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public int? SaleId { get; set; }
    public int? PurchaseOrderId { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public string? PartyPhone { get; set; }
    public string? PartyEmail { get; set; }
    public string? PartyAddress { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue => Amount - AmountPaid;
    public DateTime DueDate { get; set; }
    public int DaysUntilDue => (DueDate.Date - DateTime.UtcNow.Date).Days;
    public bool IsOverdue => BalanceDue > 0 && DueDate.Date < DateTime.UtcNow.Date;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class RecordCreditPaymentRequest
{
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCreditLedgerRequest
{
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public CreditStatus? Status { get; set; }
}
