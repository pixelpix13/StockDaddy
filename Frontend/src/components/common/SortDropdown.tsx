import React from 'react';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import type { SortState } from '@/hooks/usePagedList';

export interface SortOption {
  sortBy: string;
  sortDir: 'asc' | 'desc';
  label: string;
}

interface SortDropdownProps {
  sort: SortState;
  options: SortOption[];
  onSortChange: (sortBy: string, sortDir: 'asc' | 'desc') => void;
  className?: string;
}

/** Sort control for card/list views without table headers. */
export function SortDropdown({ sort, options, onSortChange, className = '' }: SortDropdownProps) {
  const value = `${sort.sortBy}:${sort.sortDir}`;
  const matched = options.find((o) => o.sortBy === sort.sortBy && o.sortDir === sort.sortDir);
  const display = matched?.label ?? 'Sort';

  return (
    <Select
      value={value}
      onValueChange={(v) => {
        const [sortBy, sortDir] = v.split(':') as [string, 'asc' | 'desc'];
        onSortChange(sortBy, sortDir);
      }}
    >
      <SelectTrigger className={`w-[180px] h-9 text-xs ${className}`}>
        <SelectValue placeholder="Sort">{display}</SelectValue>
      </SelectTrigger>
      <SelectContent>
        {options.map((opt) => (
          <SelectItem key={`${opt.sortBy}:${opt.sortDir}`} value={`${opt.sortBy}:${opt.sortDir}`}>
            {opt.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
