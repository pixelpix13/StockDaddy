export type CreditPartyType = 'Customer' | 'Supplier';
export type CreditStatus = 'Pending' | 'PartiallyPaid' | 'Paid' | 'Overdue';

export interface CreditLedgerDto {
  id: number;
  tenantId: number;
  partyType: CreditPartyType;
  status: CreditStatus;
  customerId?: number | null;
  supplierId?: number | null;
  saleId?: number | null;
  purchaseOrderId?: number | null;
  partyName: string;
  partyPhone?: string | null;
  partyEmail?: string | null;
  partyAddress?: string | null;
  amount: number;
  amountPaid: number;
  balanceDue: number;
  dueDate: string;
  daysUntilDue: number;
  isOverdue: boolean;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface RecordCreditPaymentRequest {
  amount: number;
  notes?: string;
}

export interface UpdateCreditLedgerRequest {
  dueDate?: string;
  notes?: string;
  status?: CreditStatus;
}

export interface CustomerSaleHistoryDto {
  id: number;
  createdAt: string;
  subtotalAmount: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  paymentMethod: string;
  items: SaleItemDetailDto[];
}

export interface SaleItemDetailDto {
  id: number;
  productVariantId: number;
  variantName: string;
  skuCode: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}
