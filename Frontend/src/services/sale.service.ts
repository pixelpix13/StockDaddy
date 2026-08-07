/** Sale and invoice read/create. POS checkout uses `orchestrationService.checkout` instead. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import { SaleDto, CreateSaleRequest, CreateSaleItemRequest, InvoiceDto } from '../dtos';

export const saleService = {
  getSalesPaged(query: PagedQuery): Promise<PagedResult<SaleDto>> {
    return fetchPaged<SaleDto>('/sale', query);
  },

  async getSales(): Promise<SaleDto[]> {
    return fetchAllItems<SaleDto>('/sale', { sortBy: 'id', sortDir: 'desc' });
  },

  async getSaleById(id: number): Promise<SaleDto> {
    const response = await apiClient.get<SaleDto>(`/sale/${id}`);
    return response.data;
  },

  async createSale(request: CreateSaleRequest): Promise<number> {
    const response = await apiClient.post<{ id: number }>('/sale', request);
    return response.data.id;
  },

  async createSaleItem(request: CreateSaleItemRequest): Promise<void> {
    await apiClient.post('/saleitem', request);
  },

  getInvoicesPaged(query: PagedQuery): Promise<PagedResult<InvoiceDto>> {
    return fetchPaged<InvoiceDto>('/invoice', query);
  },

  async getInvoices(): Promise<InvoiceDto[]> {
    return fetchAllItems<InvoiceDto>('/invoice');
  },
};
