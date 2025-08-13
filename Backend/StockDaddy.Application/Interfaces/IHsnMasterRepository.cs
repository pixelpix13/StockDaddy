using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IHsnMasterRepository
{
    Task<List<HsnMasterDto>> GetAllAsync();
    Task<HsnMasterDto?> GetByIdAsync(int id);
    Task AddAsync(CreateHsnMasterRequest hsn);
    Task UpdateAsync(int id, UpdateHsnMasterRequest hsn);
    Task DeleteAsync(int id);
}
