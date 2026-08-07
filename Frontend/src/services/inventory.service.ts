/** Stock item CRUD and restock alerts (`/api/stockitem`, `/api/productrestockalert`). */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import { StockItemDto, CreateStockItemRequest, ProductRestockAlertDto } from '../dtos';

export const inventoryService = {
  getStockItemsPaged(query: PagedQuery): Promise<PagedResult<StockItemDto>> {
    return fetchPaged<StockItemDto>('/stockitem', query);
  },

  async getStockItems(): Promise<StockItemDto[]> {
    return fetchAllItems<StockItemDto>('/stockitem');
  },

  async createStockItem(request: CreateStockItemRequest): Promise<void> {
    await apiClient.post('/stockitem', request);
  },

  async updateStockItem(id: number, request: Partial<CreateStockItemRequest>): Promise<void> {
    await apiClient.put(`/stockitem/${id}`, request);
  },

  getRestockAlertsPaged(query: PagedQuery): Promise<PagedResult<ProductRestockAlertDto>> {
    return fetchPaged<ProductRestockAlertDto>('/productrestockalert', query);
  },

  async getRestockAlerts(): Promise<ProductRestockAlertDto[]> {
    return fetchAllItems<ProductRestockAlertDto>('/productrestockalert');
  },
};
