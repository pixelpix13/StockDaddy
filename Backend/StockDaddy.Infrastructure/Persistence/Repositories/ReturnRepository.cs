using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ReturnRepository : IReturnRepository
{
    private readonly ApplicationDbContext _context;

    public ReturnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReturnDto>> GetAllAsync()
    {
        return await _context.Returns
            .Where(r => !r.IsDeleted)
            .Select(r => new ReturnDto
            {
                Id = r.Id,
                SaleId = r.SaleId,
                StoreId = r.StoreId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ReturnDto?> GetByIdAsync(int id)
    {
        return await _context.Returns
            .Where(r => r.Id == id && !r.IsDeleted)
            .Select(r => new ReturnDto
            {
                Id = r.Id,
                SaleId = r.SaleId,
                StoreId = r.StoreId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateReturnRequest returnEntity)
    {
        var entity = new Return
        {
            SaleId = returnEntity.SaleId,
            StoreId = returnEntity.StoreId,
            Reason = returnEntity.Reason,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Returns.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateReturnRequest returnEntity)
    {
        var entity = await _context.Returns.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null) return;
        entity.Reason = returnEntity.Reason;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Returns.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Returns.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Returns.Update(entity);
        await _context.SaveChangesAsync();
    }
}
