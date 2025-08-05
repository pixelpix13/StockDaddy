using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.Application.Services
{
    public class ProductRestockAlertService
    {
        private readonly IProductRestockAlertRepository _repo;

        public ProductRestockAlertService(IProductRestockAlertRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductRestockAlertDto>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<ProductRestockAlertDto?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<ProductRestockAlertDto> AddAsync(CreateProductRestockAlertRequest request)
        {
            await _repo.AddAsync(request);
            var all = await _repo.GetAllAsync();
            return all.OrderByDescending(a => a.Id).First();
        }

        public async Task<ProductRestockAlertDto?> UpdateAsync(int id, UpdateProductRestockAlertRequest request)
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
}