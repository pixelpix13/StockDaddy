import { useCallback, useEffect, useMemo, useState } from 'react';
import { ListFilters, PagedQuery, PagedResult, DEFAULT_PAGE_SIZE } from '@/types/paging';
import { useDebouncedSearch } from './useDebouncedSearch';

export interface SortState {
  sortBy: string;
  sortDir: 'asc' | 'desc';
}

interface UsePagedListOptions<T> {
  fetchFn: (query: PagedQuery) => Promise<PagedResult<T>>;
  defaultSortBy?: string;
  defaultSortDir?: 'asc' | 'desc';
  defaultPageSize?: number;
  /** When false, skip fetching (e.g. permission gate). */
  enabled?: boolean;
}

/**
 * Manages paginated list state: page, sort, debounced search, filters, and reload.
 */
export function usePagedList<T>({
  fetchFn,
  defaultSortBy = 'id',
  defaultSortDir = 'desc',
  defaultPageSize = DEFAULT_PAGE_SIZE,
  enabled = true,
}: UsePagedListOptions<T>) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [sortBy, setSortBy] = useState(defaultSortBy);
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>(defaultSortDir);
  const [filters, setFiltersState] = useState<ListFilters>({});
  const { searchInput, setSearchInput, activeSearch, commitSearch } = useDebouncedSearch();

  const [items, setItems] = useState<T[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const sort: SortState = { sortBy, sortDir };

  const load = useCallback(async () => {
    if (!enabled) return;
    setIsLoading(true);
    setError(null);
    try {
      const result = await fetchFn({
        page,
        pageSize,
        sortBy,
        sortDir,
        search: activeSearch || undefined,
        ...filters,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load data');
      setItems([]);
      setTotalCount(0);
    } finally {
      setIsLoading(false);
    }
  }, [enabled, fetchFn, page, pageSize, sortBy, sortDir, activeSearch, filters]);

  useEffect(() => {
    load();
  }, [load]);

  const toggleSort = useCallback(
    (column: string) => {
      setPage(1);
      if (sortBy === column) {
        setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'));
      } else {
        setSortBy(column);
        setSortDir('asc');
      }
    },
    [sortBy]
  );

  const handleSearchCommit = useCallback(() => {
    commitSearch();
    setPage(1);
  }, [commitSearch]);

  const handleSearchChange = useCallback(
    (value: string) => {
      setSearchInput(value);
      if (value.trim() === '') {
        setPage(1);
      }
    },
    [setSearchInput]
  );

  const setSort = useCallback((column: string, direction: 'asc' | 'desc') => {
    setPage(1);
    setSortBy(column);
    setSortDir(direction);
  }, []);

  const setFilter = useCallback(<K extends keyof ListFilters>(key: K, value: ListFilters[K] | undefined) => {
    setPage(1);
    setFiltersState((prev) => {
      const next = { ...prev };
      if (value === undefined || value === null || value === '') {
        delete next[key];
      } else {
        next[key] = value;
      }
      return next;
    });
  }, []);

  const clearFilters = useCallback(() => {
    setPage(1);
    setFiltersState({});
  }, []);

  const hasActiveFilters = useMemo(
    () => Object.values(filters).some((value) => value !== undefined && value !== ''),
    [filters]
  );

  return {
    items,
    isLoading,
    error,
    page,
    pageSize,
    totalCount,
    sort,
    filters,
    hasActiveFilters,
    searchInput,
    setPage,
    setPageSize,
    setSort,
    setFilter,
    clearFilters,
    toggleSort,
    handleSearchChange,
    handleSearchCommit,
    reload: load,
  };
}
