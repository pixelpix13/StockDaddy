import React from 'react';
import { ArrowDown, ArrowUp, ArrowUpDown } from 'lucide-react';
import type { SortState } from '@/hooks/usePagedList';

interface SortableTableHeaderProps {
  label: string;
  sortKey: string;
  sort: SortState;
  onSortChange: (sortKey: string) => void;
  className?: string;
}

/** Clickable table header cell for custom HTML tables — toggles asc/desc. */
export function SortableTableHeader({
  label,
  sortKey,
  sort,
  onSortChange,
  className = '',
}: SortableTableHeaderProps) {
  const isActive = sort.sortBy === sortKey;
  const Icon = !isActive ? ArrowUpDown : sort.sortDir === 'asc' ? ArrowUp : ArrowDown;

  return (
    <th className={`pb-3 pr-4 ${className}`}>
      <button
        type="button"
        onClick={() => onSortChange(sortKey)}
        className={`inline-flex items-center gap-1 text-left transition-colors ${
          isActive ? 'text-blue-400' : 'text-slate-400 hover:text-slate-200'
        }`}
      >
        {label}
        <Icon className="w-3.5 h-3.5 shrink-0" />
      </button>
    </th>
  );
}

export type { SortState };
