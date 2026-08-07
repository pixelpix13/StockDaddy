import React from 'react';
import { Search, X } from 'lucide-react';
import { Input } from '@/components/ui/input';

interface SearchInputProps {
  value: string;
  onChange: (value: string) => void;
  onCommit: () => void;
  placeholder?: string;
  className?: string;
  /** Shown when fewer than minLength chars typed (default 3). */
  minLength?: number;
}

/**
 * Debounced search field — parent owns debounce via useDebouncedSearch.
 * Press Enter to search immediately (even below min length).
 */
export function SearchInput({
  value,
  onChange,
  onCommit,
  placeholder = 'Search… (min 3 chars, or press Enter)',
  className = '',
  minLength = 3,
}: SearchInputProps) {
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      onCommit();
    }
  };

  return (
    <div className={`relative ${className}`}>
      <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500 pointer-events-none" />
      <Input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={handleKeyDown}
        placeholder={placeholder ?? 'Search all columns… (min 3 chars, or press Enter)'}
        className="pl-9 pr-9"
        aria-label="Search"
      />
      {value.length > 0 && (
        <button
          type="button"
          onClick={() => onChange('')}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 hover:text-slate-300"
          aria-label="Clear search"
        >
          <X className="w-4 h-4" />
        </button>
      )}
      {value.length > 0 && value.trim().length < minLength && (
        <p className="text-[10px] text-slate-500 mt-1">
          Type {minLength - value.trim().length} more character(s), or press Enter
        </p>
      )}
    </div>
  );
}
