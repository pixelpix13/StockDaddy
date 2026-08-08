import React from 'react';
import { ArrowDown, ArrowUp, ArrowUpDown } from 'lucide-react';
import type { SortState } from '@/hooks/usePagedList';

export interface Column<T> {
  header: string;
  accessor: keyof T | ((row: T) => React.ReactNode);
  /** Cell-only classes (not applied to header). */
  cellClassName?: string;
  /** @deprecated Use cellClassName */
  className?: string;
  /** Tailwind width utility, e.g. w-24, w-32, min-w-[200px]. */
  width?: string;
  align?: 'left' | 'center' | 'right';
  /** When set, column header is sortable and sends this key to the API. */
  sortKey?: string;
}

interface TableProps<T> {
  columns: Column<T>[];
  data: T[];
  keyExtractor: (row: T) => string | number;
  emptyMessage?: string;
  isLoading?: boolean;
  sort?: SortState;
  onSortChange?: (sortKey: string) => void;
  footer?: React.ReactNode;
}

const alignClass = (align?: Column<unknown>['align']) => {
  switch (align) {
    case 'center':
      return 'text-center';
    case 'right':
      return 'text-right';
    default:
      return 'text-left';
  }
};

const cellPadding = 'px-4 py-3.5 sm:px-5 sm:py-4';

export function Table<T>({
  columns,
  data,
  keyExtractor,
  emptyMessage = 'No data available',
  isLoading = false,
  sort,
  onSortChange,
  footer,
}: TableProps<T>) {
  const headerBase = `${cellPadding} text-xs font-semibold uppercase tracking-wider text-muted-foreground whitespace-nowrap`;

  const renderHeader = (col: Column<T>, index: number) => {
    const thClass = `${headerBase} ${alignClass(col.align)} ${col.width ?? ''}`.trim();

    if (col.sortKey && sort && onSortChange) {
      const isActive = sort.sortBy === col.sortKey;
      const Icon = !isActive ? ArrowUpDown : sort.sortDir === 'asc' ? ArrowUp : ArrowDown;
      return (
        <th key={index} className={thClass}>
          <button
            type="button"
            onClick={() => onSortChange(col.sortKey!)}
            className={`inline-flex items-center gap-1.5 transition-colors ${
              isActive ? 'text-blue-400' : 'text-muted-foreground hover:text-foreground'
            } ${col.align === 'right' ? 'ml-auto' : ''}`}
          >
            {col.header}
            <Icon className="w-3.5 h-3.5 shrink-0 opacity-80" />
          </button>
        </th>
      );
    }

    return (
      <th key={index} className={thClass}>
        {col.header}
      </th>
    );
  };

  return (
    <div className="w-full min-w-0 overflow-x-auto rounded-xl border border-border bg-card/40">
      <table className="w-full min-w-[640px] table-auto text-sm text-foreground/90 border-collapse">
        <thead className="bg-card/80 border-b border-border">
          <tr>{columns.map(renderHeader)}</tr>
        </thead>
        <tbody className="divide-y divide-border/60">
          {isLoading ? (
            <tr>
              <td colSpan={columns.length} className={`${cellPadding} text-center text-muted-foreground`}>
                <div className="flex items-center justify-center gap-2 py-4">
                  <div className="w-4 h-4 rounded-full border-2 border-blue-500 border-t-transparent animate-spin" />
                  <span>Loading data...</span>
                </div>
              </td>
            </tr>
          ) : data.length === 0 ? (
            <tr>
              <td colSpan={columns.length} className={`${cellPadding} text-center text-muted-foreground font-medium`}>
                <div className="py-4">{emptyMessage}</div>
              </td>
            </tr>
          ) : (
            data.map((row) => (
              <tr
                key={keyExtractor(row)}
                className="hover:bg-muted/40 transition-colors duration-150"
              >
                {columns.map((col, index) => (
                  <td
                    key={index}
                    className={`${cellPadding} align-middle ${alignClass(col.align)} ${col.width ?? ''} ${col.cellClassName ?? col.className ?? ''}`.trim()}
                  >
                    {typeof col.accessor === 'function'
                      ? col.accessor(row)
                      : (row[col.accessor] as unknown as React.ReactNode)}
                  </td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
      {footer ? (
        <div className="px-4 sm:px-6 py-4 border-t border-border bg-card/30">
          {footer}
        </div>
      ) : null}
    </div>
  );
}
