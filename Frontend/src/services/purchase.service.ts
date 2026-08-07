/** Purchase orders, suppliers, and PO line items. */
import { apiClient } from './api.client';
import {
  PurchaseOrderDto,
  CreatePurchaseOrderRequest,
  UpdatePurchaseOrderRequest,
  SupplierDto,
  CreateSupplierRequest,
  UpdateSupplierRequest,
} from '../dtos';

export const purchaseService = {
  async getPurchaseOrders(): Promise<PurchaseOrderDto[]> {
    const response = await apiClient.get<PurchaseOrderDto[]>('/purchaseorder');
    return response.data;
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

  async getSuppliers(): Promise<SupplierDto[]> {
    const response = await apiClient.get<SupplierDto[]>('/supplier');
    return response.data;
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
