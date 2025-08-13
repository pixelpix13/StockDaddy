using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IGiftOptionRepository
{
    Task<List<GiftOptionDto>> GetAllAsync();
    Task<GiftOptionDto?> GetByIdAsync(int id);
    Task AddAsync(CreateGiftOptionRequest option);
    Task UpdateAsync(int id, UpdateGiftOptionRequest option);
    Task DeleteAsync(int id);
}
