import type { FilterOption } from '@/components/common/ListFilters';
import type { StockFilter } from '@/types/paging';

export const PAYMENT_METHOD_FILTER_OPTIONS: FilterOption<string>[] = [
  { label: 'All', value: 'all' },
  { label: 'Cash', value: 'Cash' },
  { label: 'Card', value: 'Card' },
  { label: 'UPI', value: 'UPI' },
  { label: 'Bank Transfer', value: 'BankTransfer' },
];

export const PURCHASE_ORDER_STATUS_OPTIONS: FilterOption<string>[] = [
  { label: 'All', value: 'all' },
  { label: 'Pending', value: 'Pending' },
  { label: 'Paid', value: 'Paid' },
  { label: 'Unpaid', value: 'Unpaid' },
  { label: 'Delivered', value: 'Delivered' },
  { label: 'Cancelled', value: 'Cancelled' },
];

export const STOCK_FILTER_OPTIONS: FilterOption<StockFilter>[] = [
  { label: 'All', value: 'all' },
  { label: 'In Stock', value: 'in' },
  { label: 'Low', value: 'low' },
  { label: 'Out of Stock', value: 'out' },
];

export const ALERT_STATUS_OPTIONS: FilterOption<string>[] = [
  { label: 'All', value: 'all' },
  { label: 'Pending', value: 'Pending' },
  { label: 'Resolved', value: 'Resolved' },
  { label: 'Dismissed', value: 'Dismissed' },
];

export const HSN_CGST_FILTER_OPTIONS: FilterOption<number>[] = [
  { label: 'All', value: 'all' },
  { label: '0%', value: 0 },
  { label: '2.5%', value: 2.5 },
  { label: '6%', value: 6 },
  { label: '9%', value: 9 },
  { label: '14%', value: 14 },
];

export const TAX_REGION_RATE_OPTIONS: FilterOption<number>[] = [
  { label: 'All', value: 'all' },
  { label: '5%', value: 5 },
  { label: '12%', value: 12 },
  { label: '18%', value: 18 },
  { label: '28%', value: 28 },
];

export const ROLE_TYPE_OPTIONS: FilterOption<string>[] = [
  { label: 'All', value: 'all' },
  { label: 'Built-in', value: 'builtin' },
  { label: 'Custom', value: 'custom' },
];

export function buildCategoryFilterOptions(
  categories: { id: number; name: string }[]
): FilterOption<number>[] {
  return [
    { label: 'All', value: 'all' },
    ...categories.map((c) => ({ label: c.name, value: c.id })),
  ];
}

export function buildRoleFilterOptions(
  roles: { id: number; name: string }[]
): FilterOption<number>[] {
  return [
    { label: 'All', value: 'all' },
    ...roles.map((r) => ({ label: r.name, value: r.id })),
  ];
}

export function buildSupplierFilterOptions(
  suppliers: { id: number; name: string }[]
): FilterOption<number>[] {
  return [
    { label: 'All', value: 'all' },
    ...suppliers.map((s) => ({ label: s.name, value: s.id })),
  ];
}

export function buildSubcategoryFilterOptions(
  subcategories: { id: number; name: string }[]
): FilterOption<number>[] {
  return [
    { label: 'All', value: 'all' },
    ...subcategories.map((s) => ({ label: s.name, value: s.id })),
  ];
}
