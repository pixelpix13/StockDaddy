import { useCallback, useEffect, useState } from 'react';

interface UseDebouncedSearchOptions {
  /** Debounce delay in ms (default 2000). */
  debounceMs?: number;
  /** Minimum chars before auto-search via debounce (default 3). Enter always commits. */
  minLength?: number;
}

/**
 * Search input with 2s debounce (≥ minLength chars) and immediate commit on Enter.
 */
export function useDebouncedSearch(options: UseDebouncedSearchOptions = {}) {
  const debounceMs = options.debounceMs ?? 2000;
  const minLength = options.minLength ?? 3;

  const [searchInput, setSearchInput] = useState('');
  const [activeSearch, setActiveSearch] = useState('');

  const commitSearch = useCallback(
    (value?: string) => {
      const trimmed = (value ?? searchInput).trim();
      setActiveSearch(trimmed);
    },
    [searchInput]
  );

  useEffect(() => {
    const trimmed = searchInput.trim();

    if (trimmed.length === 0) {
      setActiveSearch('');
      return;
    }

    if (trimmed.length < minLength) {
      return;
    }

    const timer = window.setTimeout(() => {
      setActiveSearch(trimmed);
    }, debounceMs);

    return () => window.clearTimeout(timer);
  }, [searchInput, debounceMs, minLength]);

  const clearSearch = useCallback(() => {
    setSearchInput('');
    setActiveSearch('');
  }, []);

  return {
    searchInput,
    setSearchInput,
    activeSearch,
    commitSearch,
    clearSearch,
  };
}
