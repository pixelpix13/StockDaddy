/** Wholesale company CRUD for B2B buyers and POS company picker. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import { CompanyDto, CreateCompanyRequest, UpdateCompanyRequest } from '../dtos/company.dto';
import { CustomerSaleHistoryDto } from '../dtos/credit.dto';

export const companyService = {
  getCompaniesPaged(query: PagedQuery): Promise<PagedResult<CompanyDto>> {
    return fetchPaged<CompanyDto>('/company', query);
  },

  async getCompanies(): Promise<CompanyDto[]> {
    return fetchAllItems<CompanyDto>('/company');
  },

  async getCompanyById(id: number): Promise<CompanyDto> {
    const response = await apiClient.get<CompanyDto>(`/company/${id}`);
    return response.data;
  },

  async createCompany(request: CreateCompanyRequest): Promise<void> {
    await apiClient.post('/company', request);
  },

  async updateCompany(id: number, request: UpdateCompanyRequest): Promise<void> {
    await apiClient.put(`/company/${id}`, request);
  },

  async deleteCompany(id: number): Promise<void> {
    await apiClient.delete(`/company/${id}`);
  },

  getSalesHistory(companyId: number, query: PagedQuery): Promise<PagedResult<CustomerSaleHistoryDto>> {
    return fetchPaged(`/company/${companyId}/sales`, query);
  },
};
