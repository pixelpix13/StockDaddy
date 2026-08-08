import type { FilterOption } from '@/components/common/ListFilters';
import type { StockFilter } from '@/types/paging';

export const PAYMENT_METHOD_FILTER_OPTIONS: FilterOption<string>[] = [
  { label: 'All', value: 'all' },
  { label: 'Cash', value: 'Cash' },
  { label: 'Card', value: 'Card' },
  { label: 'UPI', value: 'UPI' },
  { label: 'Bank Transfer', value: 'BankTransfer' },
  { label: 'Credit', value: 'Credit' },
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

export const CREDIT_PARTY_TYPE_OPTIONS: FilterOption<string>[] = [
  { label: 'All parties', value: 'all' },
  { label: 'Customers (collect)', value: 'Customer' },
  { label: 'Companies (collect)', value: 'Company' },
  { label: 'Suppliers (pay)', value: 'Supplier' },
];

export const CREDIT_STATUS_OPTIONS: FilterOption<string>[] = [
  { label: 'All statuses', value: 'all' },
  { label: 'Pending', value: 'Pending' },
  { label: 'Partially paid', value: 'PartiallyPaid' },
  { label: 'Overdue', value: 'Overdue' },
  { label: 'Paid', value: 'Paid' },
];

export const ACTIVITY_ACTION_OPTIONS: FilterOption<string>[] = [
  { label: 'All actions', value: 'all' },
  { label: 'Create', value: 'Create' },
  { label: 'Update', value: 'Update' },
  { label: 'Delete', value: 'Delete' },
];

export const ACTIVITY_ENTITY_OPTIONS: FilterOption<string>[] = [
  { label: 'All entities', value: 'all' },
  { label: 'Catalog — Category', value: 'Category' },
  { label: 'Catalog — Subcategory', value: 'Subcategory' },
  { label: 'Catalog — HSN', value: 'HsnMaster' },
  { label: 'Catalog — Tax Region', value: 'TaxRegion' },
  { label: 'Product', value: 'Product' },
  { label: 'Product Variant', value: 'ProductVariant' },
  { label: 'Inventory', value: 'StockItem' },
  { label: 'Sales', value: 'Sale' },
  { label: 'Purchase Order', value: 'PurchaseOrder' },
  { label: 'Supplier', value: 'Supplier' },
  { label: 'Customer', value: 'Customer' },
  { label: 'Users', value: 'User' },
  { label: 'Access Control', value: 'Rbac' },
  { label: 'Settings — Store', value: 'Store' },
  { label: 'Orchestration', value: 'Orchestration' },
  { label: 'Bill Adjustment', value: 'BillAdjustment' },
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

export function buildUserFilterOptions(
  users: { id: number; username: string }[]
): FilterOption<number>[] {
  return [
    { label: 'All users', value: 'all' },
    ...users.map((u) => ({ label: u.username, value: u.id })),
  ];
}
