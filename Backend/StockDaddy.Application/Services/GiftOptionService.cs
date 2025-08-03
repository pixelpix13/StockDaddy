using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class GiftOptionService
{
    private readonly IGiftOptionRepository _repo;

    public GiftOptionService(IGiftOptionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<GiftOptionDto>> GetAllAsync()
    {
        var options = await _repo.GetAllAsync();
        return options.Select(o => new GiftOptionDto
        {
            Id = o.Id,
            SaleId = o.SaleId,
            IsWrapped = o.IsWrapped,
            WrapType = o.WrapType,
            Message = o.Message,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        }).ToList();
    }

    public async Task<GiftOptionDto?> GetByIdAsync(Guid id)
    {
        var o = await _repo.GetByIdAsync(id);
        if (o == null) return null;

        return new GiftOptionDto
        {
            Id = o.Id,
            SaleId = o.SaleId,
            IsWrapped = o.IsWrapped,
            WrapType = o.WrapType,
            Message = o.Message,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };
    }

    public async Task AddAsync(CreateGiftOptionRequest request)
    {
        var option = new GiftOption
        {
            SaleId = request.SaleId,
            IsWrapped = request.IsWrapped,
            WrapType = request.WrapType,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(option);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateGiftOptionRequest request)
    {
        var option = await _repo.GetByIdAsync(id);
        if (option == null) return false;

        option.IsWrapped = request.IsWrapped;
        option.WrapType = request.WrapType;
        option.Message = request.Message;
        option.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(option);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var option = await _repo.GetByIdAsync(id);
        if (option == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
