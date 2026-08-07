import { apiClient } from '@/services/api.client';
import { UpdateSaleRequest } from '@/dtos';

/** Optional removable module — calls /api/bill-adjustment/* only when enabled. */
export const billAdjustmentService = {
  async adjustSale(saleId: number, request: UpdateSaleRequest): Promise<void> {
    await apiClient.put(`/bill-adjustment/${saleId}`, request);
  },

  async voidSale(saleId: number): Promise<void> {
    await apiClient.delete(`/bill-adjustment/${saleId}`);
  },
};
