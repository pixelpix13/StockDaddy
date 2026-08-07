/** Customer CRUD for CRM / POS customer picker. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import { CustomerDto, CreateCustomerRequest, UpdateCustomerRequest } from '../dtos';

export const customerService = {
  getCustomersPaged(query: PagedQuery): Promise<PagedResult<CustomerDto>> {
    return fetchPaged<CustomerDto>('/customer', query);
  },

  async getCustomers(): Promise<CustomerDto[]> {
    return fetchAllItems<CustomerDto>('/customer');
  },

  async getCustomerById(id: number): Promise<CustomerDto> {
    const response = await apiClient.get<CustomerDto>(`/customer/${id}`);
    return response.data;
  },

  async createCustomer(request: CreateCustomerRequest): Promise<void> {
    await apiClient.post('/customer', request);
  },

  async updateCustomer(id: number, request: UpdateCustomerRequest): Promise<void> {
    await apiClient.put(`/customer/${id}`, request);
  },

  async deleteCustomer(id: number): Promise<void> {
    await apiClient.delete(`/customer/${id}`);
  },
};
