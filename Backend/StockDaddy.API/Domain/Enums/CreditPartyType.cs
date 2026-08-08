namespace StockDaddy.Domain.Enums;

/// <summary>Who the credit entry is with — customer owes us or we owe supplier.</summary>
public enum CreditPartyType
{
    Customer,
    Supplier,
    Company
}
