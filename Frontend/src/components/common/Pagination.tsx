import React from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { DEFAULT_PAGE_SIZE } from '@/types/paging';

interface PaginationProps {
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onPageSizeChange?: (pageSize: number) => void;
  pageSizeOptions?: number[];
  className?: string;
}

export function Pagination({
  page,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
  pageSizeOptions = [10, 20, 50, 100],
  className = '',
}: PaginationProps) {
  const totalPages = pageSize <= 0 ? 0 : Math.max(1, Math.ceil(totalCount / pageSize));
  const safePage = Math.min(Math.max(page, 1), totalPages);
  const start = totalCount === 0 ? 0 : (safePage - 1) * pageSize + 1;
  const end = Math.min(safePage * pageSize, totalCount);

  return (
    <div
      className={`flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between ${className}`}
    >
      <p className="text-xs sm:text-sm text-slate-500 text-center sm:text-left">
        {totalCount === 0 ? 'No results' : `Showing ${start}–${end} of ${totalCount}`}
      </p>

      <div className="flex flex-col sm:flex-row items-stretch sm:items-center justify-center sm:justify-end gap-3 sm:gap-4">
        {onPageSizeChange && (
          <div className="flex items-center justify-center sm:justify-end gap-2">
            <span className="text-xs text-slate-500 shrink-0">Rows</span>
            <Select
              value={String(pageSize)}
              onValueChange={(v) => {
                onPageSizeChange(parseInt(v, 10) || DEFAULT_PAGE_SIZE);
                onPageChange(1);
              }}
            >
              <SelectTrigger className="w-[72px] h-9 text-xs">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {pageSizeOptions.map((size) => (
                  <SelectItem key={size} value={String(size)}>
                    {size}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        )}

        <div className="flex items-center justify-center gap-2">
          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={safePage <= 1}
            onClick={() => onPageChange(safePage - 1)}
            aria-label="Previous page"
            className="h-9 w-9 p-0 shrink-0"
          >
            <ChevronLeft className="w-4 h-4" />
          </Button>

          <span className="text-xs sm:text-sm text-slate-400 min-w-[88px] text-center px-2">
            Page {safePage} / {totalPages}
          </span>

          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={safePage >= totalPages}
            onClick={() => onPageChange(safePage + 1)}
            aria-label="Next page"
            className="h-9 w-9 p-0 shrink-0"
          >
            <ChevronRight className="w-4 h-4" />
          </Button>
        </div>
      </div>
    </div>
  );
}
