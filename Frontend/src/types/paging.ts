/** Matches backend PagedQuery / PagedResult. */
export type StockFilter = 'in' | 'low' | 'out';

export interface ListFilters {
  categoryId?: number;
  status?: string;
  roleId?: number;
  paymentMethod?: string;
  stockFilter?: StockFilter;
  supplierId?: number;
  subcategoryId?: number;
  taxPercent?: number;
}

export interface PagedQuery extends ListFilters {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
  search?: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export const DEFAULT_PAGE_SIZE = 20;
export const MAX_PAGE_SIZE = 100;

export function buildPagedParams(query: PagedQuery): Record<string, string | number> {
  const params: Record<string, string | number> = {};
  if (query.page != null) params.page = query.page;
  if (query.pageSize != null) params.pageSize = query.pageSize;
  if (query.sortBy) params.sortBy = query.sortBy;
  if (query.sortDir) params.sortDir = query.sortDir;
  if (query.search) params.search = query.search;
  if (query.categoryId != null) params.categoryId = query.categoryId;
  if (query.status) params.status = query.status;
  if (query.roleId != null) params.roleId = query.roleId;
  if (query.paymentMethod) params.paymentMethod = query.paymentMethod;
  if (query.stockFilter) params.stockFilter = query.stockFilter;
  if (query.supplierId != null) params.supplierId = query.supplierId;
  if (query.subcategoryId != null) params.subcategoryId = query.subcategoryId;
  if (query.taxPercent != null) params.taxPercent = query.taxPercent;
  return params;
}
