using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class HsnMasterService
{
    private readonly IHsnMasterRepository _repo;

    public HsnMasterService(IHsnMasterRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<HsnMasterDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(h => new HsnMasterDto
        {
            Id = h.Id,
            HSNCode = h.HSNCode,
            Description = h.Description,
            CGSTPercent = h.CGSTPercent,
            SGSTPercent = h.SGSTPercent,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        }).ToList();
    }

    public async Task<HsnMasterDto?> GetByIdAsync(Guid id)
    {
        var Hsn = await _repo.GetByIdAsync(id);
        if (Hsn == null) return null;

        return new HsnMasterDto
        {
            Id = Hsn.Id,
            HSNCode = Hsn.HSNCode,
            Description = Hsn.Description,
            CGSTPercent = Hsn.CGSTPercent,
            SGSTPercent = Hsn.SGSTPercent,
            CreatedAt = Hsn.CreatedAt,
            UpdatedAt = Hsn.UpdatedAt
        };
    }

    public async Task AddAsync(CreateHsnMasterRequest request)
    {
        var Hsn = new HsnMaster
        {
            HSNCode = request.HSNCode,
            Description = request.Description,
            CGSTPercent = request.CGSTPercent,
            SGSTPercent = request.SGSTPercent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(Hsn);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateHsnMasterRequest request)
    {
        var Hsn = await _repo.GetByIdAsync(id);
        if (Hsn == null) return false;

        Hsn.Description = request.Description;
        Hsn.CGSTPercent = request.CGSTPercent;
        Hsn.SGSTPercent = request.SGSTPercent;
        Hsn.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(Hsn);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var Hsn = await _repo.GetByIdAsync(id);
        if (Hsn == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
