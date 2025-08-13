using StockDaddy.Application.DTOs;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ISaleItemRepository
{
    Task<List<SaleItemDto>> GetAllBySaleIdAsync(int saleId);
    Task<SaleItemDto?> GetByIdAsync(int id);
    Task AddAsync(CreateSaleItemRequest item);
    Task UpdateAsync(int id, UpdateSaleItemRequest item);
    Task DeleteAsync(int id);
}
