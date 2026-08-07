using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Enums;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class CreditLedgerRepository : ICreditLedgerRepository
{
    private readonly ApplicationDbContext _context;

    public CreditLedgerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CreditLedgerDto>> GetPagedAsync(PagedQuery query)
    {
        await RefreshOverdueStatusesAsync();

        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.CreditLedgers.Where(c => !c.IsDeleted);

        if (!string.IsNullOrEmpty(q.PartyType) &&
            Enum.TryParse<CreditPartyType>(q.PartyType, true, out var partyFilter))
        {
            baseQuery = baseQuery.Where(c => c.PartyType == partyFilter);
        }

        if (!string.IsNullOrEmpty(q.Status) &&
            Enum.TryParse<CreditStatus>(q.Status, true, out var statusFilter))
        {
            baseQuery = baseQuery.Where(c => c.Status == statusFilter);
        }

        if (q.CustomerId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.CustomerId == q.CustomerId.Value);
        }

        if (q.SupplierId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.SupplierId == q.SupplierId.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(c =>
                (idMatch && c.Id == searchId) ||
                (idMatch && c.SaleId == searchId) ||
                (idMatch && c.PurchaseOrderId == searchId) ||
                EF.Functions.ILike(c.PartyName, pattern) ||
                EF.Functions.ILike(c.PartyPhone ?? string.Empty, pattern) ||
                EF.Functions.ILike(c.PartyEmail ?? string.Empty, pattern) ||
                EF.Functions.ILike(c.Notes ?? string.Empty, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(MapToDto);
        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    public async Task<CreditLedgerDto?> GetByIdAsync(int id)
    {
        await RefreshOverdueStatusesAsync();
        return await _context.CreditLedgers
            .Where(c => c.Id == id && !c.IsDeleted)
            .Select(MapToDto)
            .FirstOrDefaultAsync();
    }

    public async Task<CreditLedgerDto?> RecordPaymentAsync(int id, RecordCreditPaymentRequest request)
    {
        var entity = await _context.CreditLedgers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return null;

        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("Payment amount must be greater than zero.");
        }

        entity.AmountPaid += request.Amount;
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            entity.Notes = string.IsNullOrWhiteSpace(entity.Notes)
                ? request.Notes.Trim()
                : $"{entity.Notes}\n{request.Notes.Trim()}";
        }

        entity.Status = ResolveStatus(entity.Amount, entity.AmountPaid, entity.DueDate);
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<CreditLedgerDto?> UpdateAsync(int id, UpdateCreditLedgerRequest request)
    {
        var entity = await _context.CreditLedgers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return null;

        if (request.DueDate.HasValue) entity.DueDate = request.DueDate.Value;
        if (request.Notes != null) entity.Notes = request.Notes;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        else entity.Status = ResolveStatus(entity.Amount, entity.AmountPaid, entity.DueDate);

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task RefreshOverdueStatusesAsync()
    {
        var today = DateTime.UtcNow.Date;
        var overdue = await _context.CreditLedgers
            .Where(c => !c.IsDeleted &&
                        c.AmountPaid < c.Amount &&
                        c.DueDate.Date < today &&
                        c.Status != CreditStatus.Paid)
            .ToListAsync();

        if (overdue.Count == 0) return;

        foreach (var entry in overdue)
        {
            entry.Status = CreditStatus.Overdue;
            entry.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private static CreditStatus ResolveStatus(decimal amount, decimal amountPaid, DateTime dueDate)
    {
        if (amountPaid >= amount) return CreditStatus.Paid;
        if (amountPaid > 0) return CreditStatus.PartiallyPaid;
        return dueDate.Date < DateTime.UtcNow.Date ? CreditStatus.Overdue : CreditStatus.Pending;
    }

    private static IQueryable<CreditLedger> ApplySort(IQueryable<CreditLedger> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("duedate", true) => query.OrderByDescending(c => c.DueDate),
            ("duedate", false) => query.OrderBy(c => c.DueDate),
            ("amount", true) => query.OrderByDescending(c => c.Amount),
            ("amount", false) => query.OrderBy(c => c.Amount),
            ("partyname", true) => query.OrderByDescending(c => c.PartyName),
            ("partyname", false) => query.OrderBy(c => c.PartyName),
            (_, true) => query.OrderByDescending(c => c.Id),
            _ => query.OrderBy(c => c.DueDate),
        };

    private static readonly System.Linq.Expressions.Expression<Func<CreditLedger, CreditLedgerDto>> MapToDto =
        c => new CreditLedgerDto
        {
            Id = c.Id,
            TenantId = c.TenantId,
            PartyType = c.PartyType,
            Status = c.Status,
            CustomerId = c.CustomerId,
            SupplierId = c.SupplierId,
            SaleId = c.SaleId,
            PurchaseOrderId = c.PurchaseOrderId,
            PartyName = c.PartyName,
            PartyPhone = c.PartyPhone,
            PartyEmail = c.PartyEmail,
            PartyAddress = c.PartyAddress,
            Amount = c.Amount,
            AmountPaid = c.AmountPaid,
            DueDate = c.DueDate,
            Notes = c.Notes,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
}
