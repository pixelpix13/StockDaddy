import React, { useCallback, useEffect, useState } from 'react';
import { History, Eye } from 'lucide-react';
import { activityService, userService } from '@/services';
import { AuditLogDto } from '@/dtos/activity.dto';
import { useAuth } from '@/context/AuthContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { ACTIVITY_ACTION_OPTIONS, ACTIVITY_ENTITY_OPTIONS, buildUserFilterOptions } from '@/config/list-filters';
import { formatActivityPreview, formatActivitySummary } from '@/lib/formatActivitySummary';
import { PageHeader } from '@/components/common/PageHeader';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';

function actionBadgeVariant(action: string): 'default' | 'secondary' | 'destructive' {
  switch (action) {
    case 'Create':
      return 'default';
    case 'Update':
      return 'secondary';
    case 'Delete':
      return 'destructive';
    default:
      return 'secondary';
  }
}

function formatTimestamp(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return '—';
  return date.toLocaleString();
}

export const ActivityPage: React.FC = () => {
  const { user } = useAuth();
  const isAdmin = user?.roleName?.toLowerCase() === 'admin';

  const list = usePagedList<AuditLogDto>({
    fetchFn: useCallback((query) => activityService.getActivityPaged(query), []),
    defaultSortBy: 'timestamp',
    defaultSortDir: 'desc',
  });

  const [selectedLog, setSelectedLog] = useState<AuditLogDto | null>(null);
  const [users, setUsers] = useState<{ id: number; username: string }[]>([]);

  useEffect(() => {
    if (!isAdmin) return;
    userService
      .getUsers()
      .then((rows) => setUsers(rows.map((row) => ({ id: row.id, username: row.username }))))
      .catch(() => setUsers([]));
  }, [isAdmin]);

  const selectedSummary = selectedLog ? formatActivitySummary(selectedLog) : null;

  const columns: Column<AuditLogDto>[] = [
    {
      header: 'ID',
      accessor: (row) => `#${row.id}`,
      sortKey: 'id',
      cellClassName: 'font-mono text-xs text-slate-500',
      width: 'w-20',
    },
    {
      header: 'When',
      accessor: (row) => (
        <span className="text-xs text-slate-300 whitespace-nowrap">
          {formatTimestamp(row.timestamp || row.createdAt)}
        </span>
      ),
      sortKey: 'timestamp',
      width: 'min-w-[160px]',
    },
    {
      header: 'Activity',
      accessor: (row) => (
        <div className="space-y-1 min-w-0">
          <p className="text-sm text-slate-100 leading-snug">{formatActivityPreview(row, 160)}</p>
          <div className="flex items-center gap-2 flex-wrap">
            <Badge variant={actionBadgeVariant(row.action)} className="text-[10px] px-1.5 py-0">
              {row.action}
            </Badge>
            <span className="text-[11px] text-slate-500">{row.tableName}</span>
          </div>
        </div>
      ),
      width: 'min-w-[280px]',
    },
    {
      header: '',
      accessor: (row) => (
        <Button type="button" variant="ghost" size="sm" onClick={() => setSelectedLog(row)} title="View details">
          <Eye className="w-4 h-4" />
        </Button>
      ),
      align: 'right',
      width: 'w-16',
    },
  ];

  return (
    <div className="page-stack">
      <PageHeader
        title="Activity Log"
        description={
          isAdmin
            ? 'Audit trail for all users. Admins see every CRUD operation; use the User filter to narrow results.'
            : 'Your personal audit trail of create, update, and delete operations.'
        }
        icon={<History className="w-6 h-6 text-blue-400" />}
      />

      <Card>
        <CardHeader>
          <CardTitle>Recent Activity ({list.totalCount})</CardTitle>
          {isAdmin ? (
            <CardDescription>Showing activity for all users in the system.</CardDescription>
          ) : null}
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No activity recorded yet. CRUD operations will appear here automatically."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                {isAdmin && users.length > 0 ? (
                  <FilterSelect
                    label="User"
                    options={buildUserFilterOptions(users)}
                    value={list.filters.userId}
                    onChange={(value) => list.setFilter('userId', value)}
                  />
                ) : null}
                <FilterSelect
                  label="Action"
                  options={ACTIVITY_ACTION_OPTIONS}
                  value={list.filters.status}
                  onChange={(value) => list.setFilter('status', value)}
                />
                <FilterSelect
                  label="Entity"
                  options={ACTIVITY_ENTITY_OPTIONS}
                  value={list.filters.entity}
                  onChange={(value) => list.setFilter('entity', value)}
                />
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={!!selectedLog} onOpenChange={(open) => !open && setSelectedLog(null)}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Activity #{selectedLog?.id}</DialogTitle>
          </DialogHeader>
          {selectedLog && selectedSummary ? (
            <div className="space-y-5 text-sm">
              <div className="rounded-xl border border-slate-800 bg-slate-900/60 p-4 space-y-3">
                <p className="text-base text-slate-100 leading-relaxed">{selectedSummary.sentence}</p>
                <div className="flex items-center gap-2 flex-wrap">
                  <Badge variant={actionBadgeVariant(selectedLog.action)}>{selectedLog.action}</Badge>
                  <span className="text-xs text-slate-500">
                    {formatTimestamp(selectedLog.timestamp || selectedLog.createdAt)}
                  </span>
                  <span className="text-xs text-slate-500">·</span>
                  <span className="text-xs text-slate-400">{selectedLog.tableName} #{selectedLog.recordId}</span>
                </div>
              </div>

              {selectedSummary.details.length > 0 ? (
                <div>
                  <p className="text-xs uppercase text-slate-500 mb-2">What happened</p>
                  <ul className="space-y-1.5 text-slate-300">
                    {selectedSummary.details.map((detail) => (
                      <li key={detail} className="flex gap-2">
                        <span className="text-slate-600">•</span>
                        <span>{detail}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              ) : null}

              <div>
                <p className="text-xs uppercase text-slate-500 mb-2">Technical payload</p>
                <pre className="max-h-48 overflow-auto rounded-xl border border-slate-800 bg-slate-950/80 p-4 text-xs text-slate-400 whitespace-pre-wrap break-all">
                  {selectedLog.newData || '—'}
                </pre>
              </div>
            </div>
          ) : null}
        </DialogContent>
      </Dialog>
    </div>
  );
};
