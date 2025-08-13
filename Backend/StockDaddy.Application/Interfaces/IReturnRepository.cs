using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IReturnRepository
{
    Task<List<ReturnDto>> GetAllAsync();
    Task<ReturnDto?> GetByIdAsync(int id);
    Task AddAsync(CreateReturnRequest returnEntity);
    Task UpdateAsync(int id, UpdateReturnRequest returnEntity);
    Task DeleteAsync(int id);
}
