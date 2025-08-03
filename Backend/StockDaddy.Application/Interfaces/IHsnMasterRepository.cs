using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IHsnMasterRepository
{
    Task<List<HsnMaster>> GetAllAsync();
    Task<HsnMaster?> GetByIdAsync(Guid id);
    Task AddAsync(HsnMaster hsn);
    Task UpdateAsync(HsnMaster hsn);
    Task DeleteAsync(Guid id);
}
