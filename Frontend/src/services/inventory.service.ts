import { apiClient } from './api.client';
import { StockItemDto, CreateStockItemRequest, ProductRestockAlertDto } from '../dtos';

export const inventoryService = {
  async getStockItems(): Promise<StockItemDto[]> {
    const response = await apiClient.get<StockItemDto[]>('/stockitem');
    return response.data;
  },

  async createStockItem(request: CreateStockItemRequest): Promise<void> {
    await apiClient.post('/stockitem', request);
  },

  async updateStockItem(id: number, request: Partial<CreateStockItemRequest>): Promise<void> {
    await apiClient.put(`/stockitem/${id}`, request);
  },

  async getRestockAlerts(): Promise<ProductRestockAlertDto[]> {
    const response = await apiClient.get<ProductRestockAlertDto[]>('/productrestockalert');
    return response.data;
  },
};
