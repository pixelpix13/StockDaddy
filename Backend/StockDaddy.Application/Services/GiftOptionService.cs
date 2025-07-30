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

    public async Task<GiftOptionDto?> GetByIdAsync(Guid id)
    {
        var g = await _repo.GetByIdAsync(id);
        return g == null ? null : new GiftOptionDto
        {
            Id = g.Id,
            SaleId = g.SaleId,
            IsWrapped = g.IsWrapped,
            WrapType = g.WrapType,
            Message = g.Message
        };
    }

    public async Task<Guid> CreateAsync(CreateGiftOptionRequest req)
    {
        var gift = new GiftOption
        {
            SaleId = req.SaleId,
            IsWrapped = req.IsWrapped,
            WrapType = req.WrapType,
            Message = req.Message
        };

        await _repo.AddAsync(gift);
        return gift.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateGiftOptionRequest req)
    {
        var gift = await _repo.GetByIdAsync(id);
        if (gift == null) return false;

        if (req.IsWrapped.HasValue)
            gift.IsWrapped = req.IsWrapped.Value;

        if (!string.IsNullOrWhiteSpace(req.WrapType))
            gift.WrapType = req.WrapType;

        if (req.Message != null)
            gift.Message = req.Message;

        await _repo.UpdateAsync(gift);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var gift = await _repo.GetByIdAsync(id);
        if (gift == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
