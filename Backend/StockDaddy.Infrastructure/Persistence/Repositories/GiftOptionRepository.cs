using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class GiftOptionRepository : IGiftOptionRepository
{
    private readonly ApplicationDbContext _context;

    public GiftOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GiftOptionDto>> GetAllAsync()
    {
        return await _context.GiftOptions
            .Where(g => !g.IsDeleted)
            .Select(g => new GiftOptionDto
            {
                Id = g.Id,
                SaleId = g.SaleId,
                IsWrapped = g.IsWrapped,
                WrapType = g.WrapType,
                Message = g.Message,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<GiftOptionDto?> GetByIdAsync(int id)
    {
        var g = await _context.GiftOptions.FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
        if (g == null) return null;
        return new GiftOptionDto
        {
            Id = g.Id,
            SaleId = g.SaleId,
            IsWrapped = g.IsWrapped,
            WrapType = g.WrapType,
            Message = g.Message,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt
        };
    }

    public async Task AddAsync(CreateGiftOptionRequest option)
    {
        var entity = new GiftOption
        {
            SaleId = option.SaleId,
            IsWrapped = option.IsWrapped,
            WrapType = option.WrapType,
            Message = option.Message,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.GiftOptions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateGiftOptionRequest option)
    {
        var entity = await _context.GiftOptions.FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
        if (entity == null) return;

        entity.IsWrapped = option.IsWrapped;
        entity.WrapType = option.WrapType;
        entity.Message = option.Message;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.GiftOptions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.GiftOptions.FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.GiftOptions.Update(entity);
        await _context.SaveChangesAsync();
    }
}
