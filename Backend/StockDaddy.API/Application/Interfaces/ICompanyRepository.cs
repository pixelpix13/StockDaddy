using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ICompanyRepository
{
    Task<PagedResult<CompanyDto>> GetPagedAsync(PagedQuery query);
    Task<List<CompanyDto>> GetAllAsync();
    Task<CompanyDto?> GetByIdAsync(int id);
    Task AddAsync(CreateCompanyRequest company);
    Task UpdateAsync(int id, UpdateCompanyRequest company);
    Task DeleteAsync(int id);
    Task<PagedResult<CustomerSaleHistoryDto>> GetSalesHistoryAsync(int companyId, PagedQuery query);
}
