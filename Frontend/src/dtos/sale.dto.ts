export type PaymentMethod = 'Cash' | 'Card' | 'UPI' | 'BankTransfer';

export interface SaleItemDto {
  id: number;
  saleId: number;
  productVariantId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface SaleDto {
  id: number;
  tenantId: number;
  storeId?: number;
  customerId?: number;
  soldBy: number;
  totalAmount: number;
  paymentMethod: PaymentMethod;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  saleItems?: SaleItemDto[];
}

export interface CreateSaleItemRequest {
  saleId: number;
  productVariantId: number;
  quantity: number;
  unitPrice: number;
}

export interface CreateSaleRequest {
  tenantId: number;
  storeId?: number;
  customerId?: number;
  soldBy: number;
  totalAmount: number;
  paymentMethod: PaymentMethod;
  notes?: string;
}

export interface UpdateSaleRequest {
  totalAmount: number;
  paymentMethod: PaymentMethod;
  notes?: string;
}

export interface InvoiceDto {
  id: number;
  saleId: number;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  status: string;
  fileUrl?: string;
}
