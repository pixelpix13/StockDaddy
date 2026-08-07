/**
 * HSN master tab — GST classification codes with CGST/SGST percentages.
 */
import React, { useCallback, useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { HsnMasterDto } from '@/dtos';
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
import { HSN_CGST_FILTER_OPTIONS } from '@/config/list-filters';

export function HsnTab() {
  const { showToast } = useToast();
  const list = usePagedList<HsnMasterDto>({
    fetchFn: useCallback((query) => catalogService.getHsnCodesPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<HsnMasterDto | null>(null);
  const [hsnCode, setHsnCode] = useState('');
  const [description, setDescription] = useState('');
  const [cgstPercent, setCgstPercent] = useState('9');
  const [sgstPercent, setSgstPercent] = useState('9');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openCreate = () => {
    setEditing(null);
    setHsnCode('');
    setDescription('');
    setCgstPercent('9');
    setSgstPercent('9');
    setDialogOpen(true);
  };

  const openEdit = (row: HsnMasterDto) => {
    setEditing(row);
    setHsnCode(row.hsnCode);
    setDescription(row.description);
    setCgstPercent(String(row.cgstPercent));
    setSgstPercent(String(row.sgstPercent));
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!description.trim()) return;
    setIsSubmitting(true);
    try {
      const cgst = parseFloat(cgstPercent) || 0;
      const sgst = parseFloat(sgstPercent) || 0;
      if (editing) {
        await catalogService.updateHsnCode(editing.id, {
          description: description.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Updated', 'HSN code updated.');
      } else {
        if (!hsnCode.trim()) return;
        await catalogService.createHsnCode({
          hsnCode: hsnCode.trim(),
          description: description.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Created', 'HSN code created.');
      }
      setDialogOpen(false);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save HSN code.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: HsnMasterDto) => {
    if (!confirm(`Delete HSN "${row.hsnCode}"?`)) return;
    try {
      await catalogService.deleteHsnCode(row.id);
      showToast('success', 'Deleted', 'HSN code removed.');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete HSN code.'));
    }
  };

  const columns: Column<HsnMasterDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-slate-500' },
    { header: 'HSN Code', accessor: 'hsnCode', sortKey: 'name', className: 'font-medium text-slate-100' },
    { header: 'Description', accessor: 'description', className: 'text-slate-400' },
    {
      header: 'CGST %',
      accessor: (row) => `${row.cgstPercent}%`,
      className: 'text-emerald-400',
    },
    {
      header: 'SGST %',
      accessor: (row) => `${row.sgstPercent}%`,
      className: 'text-emerald-400',
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
            <CardTitle>HSN Master ({list.totalCount})</CardTitle>
            <CardDescription>GST tax codes for product classification</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add HSN
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No HSN codes yet."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="CGST"
                  options={HSN_CGST_FILTER_OPTIONS}
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
            <DialogTitle>{editing ? 'Edit HSN Code' : 'Add HSN Code'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>HSN Code</Label>
              <Input
                value={hsnCode}
                onChange={(e) => setHsnCode(e.target.value)}
                required={!editing}
                disabled={!!editing}
              />
            </div>
            <div className="space-y-2">
              <Label>Description</Label>
              <Input value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>CGST %</Label>
                <Input type="number" step="0.01" value={cgstPercent} onChange={(e) => setCgstPercent(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>SGST %</Label>
                <Input type="number" step="0.01" value={sgstPercent} onChange={(e) => setSgstPercent(e.target.value)} />
              </div>
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
