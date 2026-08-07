import { ProductDto, ProductVariantDto } from './product.dto';
import { PaymentMethod } from './sale.dto';

export interface VariantStockDto {
  id: number;
  productId: number;
  productName: string;
  storeId: number;
  variantName: string;
  skuCode: string;
  price: number;
  costPrice: number;
  taxPercent: number;
  hsnCodeId: number;
  quantity: number;
  subcategoryId?: number;
  subcategoryName?: string;
}

export interface CreateProductWithVariantRequest {
  tenantId: number;
  storeId: number;
  subcategoryId?: number;
  name: string;
  description?: string;
  unit?: string;
  skuCode: string;
  hsnCodeId?: number;
  costPrice: number;
  price: number;
  taxPercent?: number;
  initialQuantity: number;
}

export interface ProductWithVariantResponse {
  product: ProductDto;
  variant: ProductVariantDto;
}

export interface CheckoutLineRequest {
  productVariantId: number;
  quantity: number;
}

export interface CheckoutSaleRequest {
  tenantId: number;
  storeId: number;
  soldBy: number;
  customerId?: number;
  paymentMethod: PaymentMethod;
  notes?: string;
  items: CheckoutLineRequest[];
}

export interface CheckoutLineResponse {
  productVariantId: number;
  variantName: string;
  skuCode: string;
  quantity: number;
  unitPrice: number;
  lineSubtotal: number;
  taxAmount: number;
  lineTotal: number;
}

export interface CheckoutSaleResponse {
  saleId: number;
  subtotal: number;
  taxAmount: number;
  totalAmount: number;
  paymentMethod: PaymentMethod;
  items: CheckoutLineResponse[];
}

export interface AdjustStockRequest {
  productVariantId: number;
  quantityChange: number;
  reason?: string;
}

export interface AdjustStockResponse {
  productVariantId: number;
  variantName: string;
  skuCode: string;
  previousQuantity: number;
  newQuantity: number;
}

export interface CreatePurchaseOrderLineRequest {
  productVariantId: number;
  quantity: number;
  unitCost: number;
}

import { PurchaseOrderStatus } from './purchase.dto';

export interface CreatePurchaseOrderWithItemsRequest {
  tenantId: number;
  supplierId: number;
  storeId: number;
  orderDate: string;
  expectedDelivery: string;
  status?: PurchaseOrderStatus;
  notes?: string;
  items: CreatePurchaseOrderLineRequest[];
}

export interface PurchaseOrderWithItemsResponse {
  order: import('./purchase.dto').PurchaseOrderDto;
  items: import('./purchase.dto').PurchaseItemDto[];
}
