/** Purchase orders, suppliers, and PO line items. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import {
  PurchaseOrderDto,
  CreatePurchaseOrderRequest,
  UpdatePurchaseOrderRequest,
  SupplierDto,
  CreateSupplierRequest,
  UpdateSupplierRequest,
} from '../dtos';

export const purchaseService = {
  getPurchaseOrdersPaged(query: PagedQuery): Promise<PagedResult<PurchaseOrderDto>> {
    return fetchPaged<PurchaseOrderDto>('/purchaseorder', query);
  },

  async getPurchaseOrders(): Promise<PurchaseOrderDto[]> {
    return fetchAllItems<PurchaseOrderDto>('/purchaseorder', { sortBy: 'id', sortDir: 'desc' });
  },

  async getPurchaseOrderById(id: number): Promise<PurchaseOrderDto> {
    const response = await apiClient.get<PurchaseOrderDto>(`/purchaseorder/${id}`);
    return response.data;
  },

  async createPurchaseOrder(request: CreatePurchaseOrderRequest): Promise<void> {
    await apiClient.post('/purchaseorder', request);
  },

  async updatePurchaseOrder(id: number, request: UpdatePurchaseOrderRequest): Promise<void> {
    await apiClient.put(`/purchaseorder/${id}`, request);
  },

  async deletePurchaseOrder(id: number): Promise<void> {
    await apiClient.delete(`/purchaseorder/${id}`);
  },

  getSuppliersPaged(query: PagedQuery): Promise<PagedResult<SupplierDto>> {
    return fetchPaged<SupplierDto>('/supplier', query);
  },

  async getSuppliers(): Promise<SupplierDto[]> {
    return fetchAllItems<SupplierDto>('/supplier');
  },

  async getSupplierById(id: number): Promise<SupplierDto> {
    const response = await apiClient.get<SupplierDto>(`/supplier/${id}`);
    return response.data;
  },

  async createSupplier(request: CreateSupplierRequest): Promise<void> {
    await apiClient.post('/supplier', request);
  },

  async updateSupplier(id: number, request: UpdateSupplierRequest): Promise<void> {
    await apiClient.put(`/supplier/${id}`, request);
  },

  async deleteSupplier(id: number): Promise<void> {
    await apiClient.delete(`/supplier/${id}`);
  },
};
