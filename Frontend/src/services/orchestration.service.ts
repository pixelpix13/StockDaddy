/**
 * Multi-step backend workflows (single transaction on server).
 * Use for POS checkout, stock adjust, product+variant create, PO with line items.
 */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged, normalizePagedResult } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import {
  VariantStockDto,
  CreateProductWithVariantRequest,
  ProductWithVariantResponse,
  CheckoutSaleRequest,
  CheckoutSaleResponse,
  AdjustStockRequest,
  AdjustStockResponse,
  CreatePurchaseOrderWithItemsRequest,
  PurchaseOrderWithItemsResponse,
  UpdatePurchaseOrderWithItemsRequest,
  ReceivePurchaseOrderRequest,
} from '../dtos/orchestration.dto';

export const orchestrationService = {
  getVariantStockPaged(
    query: PagedQuery,
    storeId?: number
  ): Promise<PagedResult<VariantStockDto>> {
    return fetchPaged<VariantStockDto>(
      '/orchestration/variant-stock',
      query,
      storeId != null ? { storeId } : undefined
    );
  },

  async getVariantStock(storeId?: number): Promise<VariantStockDto[]> {
    const result = await fetchPaged<VariantStockDto>(
      '/orchestration/variant-stock',
      { page: 1, pageSize: 100 },
      storeId != null ? { storeId } : undefined
    );
    return result.items;
  },

  async getVariantByBarcode(code: string, storeId: number): Promise<VariantStockDto> {
    const response = await apiClient.get<VariantStockDto>('/orchestration/variant-by-barcode', {
      params: { code, storeId },
    });
    return response.data;
  },

  async createProductWithVariant(
    request: CreateProductWithVariantRequest
  ): Promise<ProductWithVariantResponse> {
    const response = await apiClient.post<ProductWithVariantResponse>(
      '/orchestration/product-with-variant',
      request
    );
    return response.data;
  },

  async checkout(request: CheckoutSaleRequest): Promise<CheckoutSaleResponse> {
    const response = await apiClient.post<CheckoutSaleResponse>('/orchestration/checkout', request);
    return response.data;
  },

  async adjustStock(request: AdjustStockRequest): Promise<AdjustStockResponse> {
    const response = await apiClient.post<AdjustStockResponse>(
      '/orchestration/adjust-stock',
      request
    );
    return response.data;
  },

  async createPurchaseOrderWithItems(
    request: CreatePurchaseOrderWithItemsRequest
  ): Promise<PurchaseOrderWithItemsResponse> {
    const response = await apiClient.post<PurchaseOrderWithItemsResponse>(
      '/orchestration/purchase-order-with-items',
      request
    );
    return response.data;
  },

  async getPurchaseOrderWithItems(id: number): Promise<PurchaseOrderWithItemsResponse> {
    const response = await apiClient.get<PurchaseOrderWithItemsResponse>(
      `/orchestration/purchase-order/${id}`
    );
    return response.data;
  },

  async updatePurchaseOrderWithItems(
    id: number,
    request: UpdatePurchaseOrderWithItemsRequest
  ): Promise<PurchaseOrderWithItemsResponse> {
    const response = await apiClient.put<PurchaseOrderWithItemsResponse>(
      `/orchestration/purchase-order/${id}`,
      request
    );
    return response.data;
  },

  async receivePurchaseOrder(
    id: number,
    request: ReceivePurchaseOrderRequest
  ): Promise<PurchaseOrderWithItemsResponse> {
    const response = await apiClient.post<PurchaseOrderWithItemsResponse>(
      `/orchestration/purchase-order/${id}/receive`,
      request
    );
    return response.data;
  },
};
