import React from 'react';
import type { Column } from './Table';

/** Standard ID column — first column in every table. */
export function idColumn<T extends { id: number }>(): Column<T> {
  return {
    header: 'ID',
    sortKey: 'id',
    width: 'w-24',
    cellClassName: 'font-mono text-xs text-muted-foreground tabular-nums',
    accessor: (row) => `#${row.id}`,
  };
}

/** Standard right-aligned actions column — last column in CRUD tables. */
export function actionsColumn<T>(render: (row: T) => React.ReactNode): Column<T> {
  return {
    header: 'Actions',
    align: 'right',
    width: 'w-32',
    cellClassName: 'whitespace-nowrap',
    accessor: render,
  };
}

/** Primary text column (name, title, etc.). */
export function primaryTextColumn<T>(
  header: string,
  accessor: keyof T | ((row: T) => React.ReactNode),
  sortKey?: string
): Column<T> {
  return {
    header,
    sortKey,
    cellClassName: 'font-medium text-foreground',
    accessor,
  };
}
