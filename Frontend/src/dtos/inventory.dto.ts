export type StockStatus = 'InStock' | 'Low' | 'OutOfStock' | 'Discontinued';

export interface StockItemDto {
  id: number;
  productId: number;
  storeId?: number;
  quantity: number;
  status: StockStatus;
  lastUpdated: string;
  updatedBy?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateStockItemRequest {
  productId: number;
  storeId?: number;
  quantity: number;
  status?: StockStatus;
  updatedBy?: number;
}

export interface ProductRestockAlertDto {
  id: number;
  productId: number;
  storeId: number;
  variantId: number;
  triggeredAt: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}
