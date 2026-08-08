/**
 * Tax regions tab — store-scoped regional tax rates.
 */
import React, { useCallback, useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { TaxRegionDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { usePagedList } from '@/hooks/usePagedList';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { TAX_REGION_RATE_OPTIONS } from '@/config/list-filters';

interface TaxRegionsTabProps {
  tenantId: number;
  storeId: number;
}

export function TaxRegionsTab({ tenantId, storeId }: TaxRegionsTabProps) {
  const { showToast } = useToast();
  const list = usePagedList<TaxRegionDto>({
    fetchFn: useCallback((query) => catalogService.getTaxRegionsPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<TaxRegionDto | null>(null);
  const [regionName, setRegionName] = useState('');
  const [taxPercent, setTaxPercent] = useState('18');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openCreate = () => {
    setEditing(null);
    setRegionName('');
    setTaxPercent('18');
    setDialogOpen(true);
  };

  const openEdit = (row: TaxRegionDto) => {
    setEditing(row);
    setRegionName(row.regionName);
    setTaxPercent(String(row.taxPercent));
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!regionName.trim()) return;
    setIsSubmitting(true);
    try {
      const percent = parseFloat(taxPercent) || 0;
      if (editing) {
        await catalogService.updateTaxRegion(editing.id, {
          regionName: regionName.trim(),
          taxPercent: percent,
        });
        showToast('success', 'Updated', 'Tax region updated.');
      } else {
        await catalogService.createTaxRegion({
          tenantId,
          storeId,
          regionName: regionName.trim(),
          taxPercent: percent,
        });
        showToast('success', 'Created', 'Tax region created.');
      }
      setDialogOpen(false);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save tax region.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: TaxRegionDto) => {
    if (!confirm(`Delete tax region "${row.regionName}"?`)) return;
    try {
      await catalogService.deleteTaxRegion(row.id);
      showToast('success', 'Deleted', 'Tax region removed.');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete tax region.'));
    }
  };

  const columns: Column<TaxRegionDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-muted-foreground' },
    { header: 'Region', accessor: 'regionName', sortKey: 'name', className: 'font-medium text-foreground' },
    {
      header: 'Tax %',
      accessor: (row) => `${row.taxPercent}%`,
      className: 'text-emerald-400',
    },
    {
      header: 'Store',
      accessor: (row) => (row.storeId ? `#${row.storeId}` : 'All'),
      className: 'text-xs text-muted-foreground',
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions module={APP_MODULES.Catalog} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
      ),
    },
  ];

  return (
    <>
      <Card>
        <CardHeader className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Tax Regions ({list.totalCount})</CardTitle>
            <CardDescription>Regional tax rates for store #{storeId}</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add Region
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No tax regions yet."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Tax rate"
                  options={TAX_REGION_RATE_OPTIONS}
                  value={list.filters.taxPercent}
                  onChange={(value) => list.setFilter('taxPercent', value)}
                />
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Tax Region' : 'Add Tax Region'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>Region Name</Label>
              <Input value={regionName} onChange={(e) => setRegionName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Tax Percent</Label>
              <Input type="number" step="0.01" value={taxPercent} onChange={(e) => setTaxPercent(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>{editing ? 'Update' : 'Create'}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
