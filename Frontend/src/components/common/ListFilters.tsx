import React from 'react';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { cn } from '@/lib/utils';

export interface FilterOption<T = string | number> {
  label: string;
  value: T | 'all';
}

interface FilterSelectProps<T extends string | number> {
  label?: string;
  options: FilterOption<T>[];
  value: T | undefined;
  onChange: (value: T | undefined) => void;
  className?: string;
  placeholder?: string;
}

/** Compact dropdown filter for list pages. */
export function FilterSelect<T extends string | number>({
  label,
  options,
  value,
  onChange,
  className,
  placeholder = 'All',
}: FilterSelectProps<T>) {
  const selectedStr = value == null ? 'all' : String(value);

  const handleChange = (raw: string) => {
    if (raw === 'all') {
      onChange(undefined);
      return;
    }

    const option = options.find((o) => String(o.value) === raw);
    if (!option || option.value === 'all') {
      onChange(undefined);
      return;
    }

    onChange(option.value as T);
  };

  return (
    <div className={cn('flex flex-col gap-1.5 w-full sm:w-auto sm:min-w-[160px]', className)}>
      {label ? (
        <Label className="text-xs font-semibold uppercase tracking-wide text-slate-500">{label}</Label>
      ) : null}
      <Select value={selectedStr} onValueChange={handleChange}>
        <SelectTrigger className="h-9 w-full sm:w-[180px]">
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent>
          {options.map((option) => (
            <SelectItem key={String(option.value)} value={String(option.value)}>
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}

interface ClearFiltersButtonProps {
  visible?: boolean;
  onClick: () => void;
  className?: string;
}

export function ClearFiltersButton({ visible, onClick, className }: ClearFiltersButtonProps) {
  if (!visible) return null;

  return (
    <Button type="button" size="sm" variant="ghost" onClick={onClick} className={className}>
      Clear filters
    </Button>
  );
}

interface ListFilterBarProps {
  children: React.ReactNode;
  onClear?: () => void;
  showClear?: boolean;
  className?: string;
}

/** Wraps one or more filter dropdowns with optional clear action. */
export function ListFilterBar({ children, onClear, showClear, className }: ListFilterBarProps) {
  return (
    <div
      className={cn(
        'flex flex-col sm:flex-row sm:flex-wrap sm:items-end gap-3 sm:gap-4',
        className
      )}
    >
      {children}
      {showClear && onClear ? (
        <ClearFiltersButton visible onClick={onClear} className="self-start sm:self-auto sm:mb-0.5" />
      ) : null}
    </div>
  );
}
