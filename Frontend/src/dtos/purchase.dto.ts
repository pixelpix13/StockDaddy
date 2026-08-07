export type PurchaseOrderStatus =
  | 'Pending'
  | 'Paid'
  | 'Unpaid'
  | 'Overdue'
  | 'Cancelled'
  | 'Delivered'
  | 'Failed';

export interface SupplierDto {
  id: number;
  tenantId: number;
  name: string;
  contactName?: string;
  email?: string;
  phone?: string;
  address?: string;
  createdAt: string;
  updatedAt?: string;
  isDeleted?: boolean;
  deletedAt?: string;
}

export interface CreateSupplierRequest {
  tenantId: number;
  name: string;
  contactName?: string;
  email?: string;
  phone?: string;
  address?: string;
}

export interface PurchaseItemDto {
  id: number;
  purchaseOrderId: number;
  productVariantId: number;
  quantity: number;
  quantityReceived?: number | null;
  unitCost: number;
  totalCost: number;
  productName?: string;
  skuCode?: string;
}

export interface PurchaseOrderDto {
  id: number;
  tenantId: number;
  supplierId: number;
  storeId: number;
  orderDate: string;
  expectedDelivery: string;
  status: PurchaseOrderStatus;
  totalAmount?: number;
  dueDate?: string | null;
  notes: string;
  fullyReceived?: boolean;
  createdAt: string;
  updatedAt: string;
  items?: PurchaseItemDto[];
}

export interface UpdateSupplierRequest {
  name: string;
  contactName?: string;
  email?: string;
  phone?: string;
  address?: string;
}

export interface UpdatePurchaseOrderRequest {
  expectedDelivery: string;
  status: PurchaseOrderStatus;
  notes?: string;
}

export interface CreatePurchaseOrderRequest {
  tenantId: number;
  supplierId: number;
  storeId: number;
  orderDate: string;
  expectedDelivery: string;
  status?: PurchaseOrderStatus;
  notes?: string;
}
