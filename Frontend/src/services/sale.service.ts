import { apiClient } from './api.client';
import { SaleDto, CreateSaleRequest, CreateSaleItemRequest, InvoiceDto } from '../dtos';

export const saleService = {
  async getSales(): Promise<SaleDto[]> {
    const response = await apiClient.get<SaleDto[]>('/sale');
    return response.data;
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

  async getInvoices(): Promise<InvoiceDto[]> {
    const response = await apiClient.get<InvoiceDto[]>('/invoice');
    return response.data;
  },
};
