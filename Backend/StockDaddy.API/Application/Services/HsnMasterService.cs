using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
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
        return await _repo.GetAllAsync();
    }

    public async Task<HsnMasterDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<HsnMasterDto> AddAsync(CreateHsnMasterRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(h => h.Id).First();
    }

    public async Task<HsnMasterDto?> UpdateAsync(int id, UpdateHsnMasterRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}
