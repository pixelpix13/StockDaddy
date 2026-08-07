import React from 'react';

import { SearchInput } from './SearchInput';



interface ListToolbarProps {

  searchInput: string;

  onSearchChange: (value: string) => void;

  onSearchCommit: () => void;

  searchPlaceholder?: string;

  children?: React.ReactNode;

  filters?: React.ReactNode;

  className?: string;

}



/** Search bar + optional filters and action buttons for list pages. */

export function ListToolbar({

  searchInput,

  onSearchChange,

  onSearchCommit,

  searchPlaceholder,

  children,

  filters,

  className = '',

}: ListToolbarProps) {

  return (

    <div className={`space-y-4 ${className}`}>

      <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-3 sm:gap-4">

        <SearchInput

          value={searchInput}

          onChange={onSearchChange}

          onCommit={onSearchCommit}

          placeholder={searchPlaceholder}

          className="w-full lg:max-w-sm"

        />

        {children ? (

          <div className="flex flex-wrap items-center gap-2 shrink-0">{children}</div>

        ) : null}

      </div>

      {filters ? <div className="pt-1">{filters}</div> : null}

    </div>

  );

}



export { SearchInput } from './SearchInput';

export { SortableTableHeader } from './SortableTableHeader';

