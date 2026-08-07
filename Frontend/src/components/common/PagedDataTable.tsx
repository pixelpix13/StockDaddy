import React from 'react';
import { Table, Column } from './Table';
import { ListToolbar } from './ListToolbar';
import { Pagination } from './Pagination';
import type { SortState } from '@/hooks/usePagedList';
import type { ListFilters } from '@/types/paging';

/** Subset of usePagedList return value needed for data tables. */
export interface PagedListBindings<T> {
  items: T[];
  isLoading: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  sort: SortState;
  searchInput: string;
  filters?: ListFilters;
  hasActiveFilters?: boolean;
  setPage: (page: number) => void;
  setPageSize: (size: number) => void;
  toggleSort: (sortKey: string) => void;
  handleSearchChange: (value: string) => void;
  handleSearchCommit: () => void;
  setFilter?: <K extends keyof ListFilters>(key: K, value: ListFilters[K] | undefined) => void;
  clearFilters?: () => void;
}

interface PagedDataTableProps<T> {
  columns: Column<T>[];
  list: PagedListBindings<T>;
  keyExtractor: (row: T) => string | number;
  emptyMessage?: string;
  searchPlaceholder?: string;
  toolbarExtra?: React.ReactNode;
  filters?: React.ReactNode;
  pageSizeOptions?: number[];
}

/**
 * Standard list layout: search toolbar + sortable table + pagination footer.
 */
export function PagedDataTable<T>({
  columns,
  list,
  keyExtractor,
  emptyMessage = 'No data available',
  searchPlaceholder,
  toolbarExtra,
  filters,
  pageSizeOptions,
}: PagedDataTableProps<T>) {
  return (
    <div className="space-y-4 sm:space-y-5 min-w-0">
      <ListToolbar
        searchInput={list.searchInput}
        onSearchChange={list.handleSearchChange}
        onSearchCommit={list.handleSearchCommit}
        searchPlaceholder={searchPlaceholder}
        filters={filters}
      >
        {toolbarExtra}
      </ListToolbar>

      <Table
        columns={columns}
        data={list.items}
        keyExtractor={keyExtractor}
        isLoading={list.isLoading}
        emptyMessage={emptyMessage}
        sort={list.sort}
        onSortChange={list.toggleSort}
        footer={
          <Pagination
            page={list.page}
            pageSize={list.pageSize}
            totalCount={list.totalCount}
            onPageChange={list.setPage}
            onPageSizeChange={list.setPageSize}
            pageSizeOptions={pageSizeOptions}
          />
        }
      />
    </div>
  );
}

export type { Column };
