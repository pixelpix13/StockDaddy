import { apiClient } from './api.client';
import { CustomerDto, CreateCustomerRequest, UpdateCustomerRequest } from '../dtos';

export const customerService = {
  async getCustomers(): Promise<CustomerDto[]> {
    const response = await apiClient.get<CustomerDto[]>('/customer');
    return response.data;
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
