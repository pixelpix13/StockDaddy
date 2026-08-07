/** Tenant, store, and role lookups for settings screens. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import { TenantDto, StoreDto, RoleDto, CreateStoreRequest, UpdateStoreRequest } from '../dtos';

export const tenantService = {
  getTenantsPaged(query: PagedQuery): Promise<PagedResult<TenantDto>> {
    return fetchPaged<TenantDto>('/tenant', query);
  },

  async getTenants(): Promise<TenantDto[]> {
    return fetchAllItems<TenantDto>('/tenant');
  },

  getStoresPaged(query: PagedQuery): Promise<PagedResult<StoreDto>> {
    return fetchPaged<StoreDto>('/store', query);
  },

  async getStores(): Promise<StoreDto[]> {
    return fetchAllItems<StoreDto>('/store');
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
    return fetchAllItems<RoleDto>('/role');
  },
};
