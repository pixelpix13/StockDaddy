using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class HsnMasterRepository : IHsnMasterRepository
{
    private readonly ApplicationDbContext _context;

    public HsnMasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HsnMasterDto>> GetAllAsync()
    {
        return await _context.HsnMaster
            .Where(h => !h.IsDeleted)
            .Select(h => new HsnMasterDto
            {
                Id = h.Id,
                HSNCode = h.HSNCode,
                Description = h.Description,
                CGSTPercent = h.CGSTPercent,
                SGSTPercent = h.SGSTPercent,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<HsnMasterDto?> GetByIdAsync(int id)
    {
        var h = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (h == null) return null;
        return new HsnMasterDto
        {
            Id = h.Id,
            HSNCode = h.HSNCode,
            Description = h.Description,
            CGSTPercent = h.CGSTPercent,
            SGSTPercent = h.SGSTPercent,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        };
    }

    public async Task AddAsync(CreateHsnMasterRequest hsn)
    {
        var entity = new HsnMaster
        {
            HSNCode = hsn.HSNCode,
            Description = hsn.Description,
            CGSTPercent = hsn.CGSTPercent,
            SGSTPercent = hsn.SGSTPercent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.HsnMaster.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateHsnMasterRequest hsn)
    {
        var entity = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (entity == null) return;

        entity.Description = hsn.Description;
        entity.CGSTPercent = hsn.CGSTPercent;
        entity.SGSTPercent = hsn.SGSTPercent;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.HsnMaster.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.HsnMaster.Update(entity);
        await _context.SaveChangesAsync();
    }
}
