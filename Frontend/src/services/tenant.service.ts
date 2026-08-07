import { apiClient } from './api.client';
import { TenantDto, StoreDto, RoleDto, CreateStoreRequest, UpdateStoreRequest } from '../dtos';

export const tenantService = {
  async getTenants(): Promise<TenantDto[]> {
    const response = await apiClient.get<TenantDto[]>('/tenant');
    return response.data;
  },

  async getStores(): Promise<StoreDto[]> {
    const response = await apiClient.get<StoreDto[]>('/store');
    return response.data;
  },

  async createStore(request: CreateStoreRequest): Promise<void> {
    await apiClient.post('/store', request);
  },

  async updateStore(id: number, request: UpdateStoreRequest): Promise<void> {
    await apiClient.put(`/store/${id}`, request);
  },

  async deleteStore(id: number): Promise<void> {
    await apiClient.delete(`/store/${id}`);
  },

  async getRoles(): Promise<RoleDto[]> {
    const response = await apiClient.get<RoleDto[]>('/role');
    return response.data;
  },
};
