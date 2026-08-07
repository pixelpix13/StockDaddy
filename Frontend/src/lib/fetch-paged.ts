import { apiClient } from '@/services/api.client';
import { buildPagedParams, DEFAULT_PAGE_SIZE, PagedQuery, PagedResult } from '@/types/paging';

type RawPagedResult<T> = PagedResult<T> & {
  Items?: T[];
  Page?: number;
  PageSize?: number;
  TotalCount?: number;
  TotalPages?: number;
  HasNext?: boolean;
  HasPrevious?: boolean;
};

/** Normalize ASP.NET JSON (camelCase or PascalCase) into PagedResult. */
export function normalizePagedResult<T>(data: RawPagedResult<T>): PagedResult<T> {
  const items = data.items ?? data.Items ?? [];
  const page = data.page ?? data.Page ?? 1;
  const pageSize = data.pageSize ?? data.PageSize ?? DEFAULT_PAGE_SIZE;
  const totalCount = data.totalCount ?? data.TotalCount ?? items.length;
  const totalPages =
    data.totalPages ??
    data.TotalPages ??
    (pageSize <= 0 ? 0 : Math.ceil(totalCount / pageSize));

  return {
    items,
    page,
    pageSize,
    totalCount,
    totalPages,
    hasNext: data.hasNext ?? data.HasNext ?? page < totalPages,
    hasPrevious: data.hasPrevious ?? data.HasPrevious ?? page > 1,
  };
}

/** Fetch a paginated list endpoint. */
export async function fetchPaged<T>(
  path: string,
  query: PagedQuery = { page: 1, pageSize: DEFAULT_PAGE_SIZE },
  extraParams?: Record<string, string | number | boolean | undefined>
): Promise<PagedResult<T>> {
  const params: Record<string, string | number> = {
    ...buildPagedParams(query),
  };
  if (extraParams) {
    for (const [key, value] of Object.entries(extraParams)) {
      if (value !== undefined && value !== null) {
        params[key] = typeof value === 'boolean' ? (value ? 1 : 0) : value;
      }
    }
  }
  const response = await apiClient.get<RawPagedResult<T>>(path, { params });
  return normalizePagedResult(response.data);
}

/** Fetch all items for dropdowns (uses max page size). */
export async function fetchAllItems<T>(
  path: string,
  query: Omit<PagedQuery, 'page' | 'pageSize'> = {}
): Promise<T[]> {
  const result = await fetchPaged<T>(path, { ...query, page: 1, pageSize: 100 });
  return result.items;
}
