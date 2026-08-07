import React, { useCallback, useState } from 'react';
import { Building2, Plus } from 'lucide-react';
import { purchaseService } from '@/services';
import { SupplierDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { useAuth } from '@/context/AuthContext';
import { usePagedList } from '@/hooks/usePagedList';
import { ListToolbar } from '@/components/common/ListToolbar';
import { Table, Column } from '@/components/common/Table';
import { Pagination } from '@/components/common/Pagination';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { getApiErrorMessage } from '@/lib/api-error';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';

export const SuppliersPage: React.FC = () => {
  const list = usePagedList<SupplierDto>({
    fetchFn: useCallback((query) => purchaseService.getSuppliersPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<SupplierDto | null>(null);
  const [name, setName] = useState('');
  const [contactName, setContactName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { showToast } = useToast();
  const { user } = useAuth();

  const resetForm = () => {
    setName('');
    setContactName('');
    setEmail('');
    setPhone('');
    setAddress('');
    setEditing(null);
  };

  const openCreate = () => {
    resetForm();
    setDialogOpen(true);
  };

  const openEdit = (supplier: SupplierDto) => {
    setEditing(supplier);
    setName(supplier.name);
    setContactName(supplier.contactName || '');
    setEmail(supplier.email || '');
    setPhone(supplier.phone || '');
    setAddress(supplier.address || '');
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    setIsSubmitting(true);
    try {
      const payload = { name, contactName, email, phone, address };
      if (editing) {
        await purchaseService.updateSupplier(editing.id, payload);
        showToast('success', 'Updated', `Supplier "${name}" updated.`);
      } else {
        await purchaseService.createSupplier({ tenantId: user?.tenantId || 1, ...payload });
        showToast('success', 'Created', `Supplier "${name}" created.`);
      }
      setDialogOpen(false);
      resetForm();
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save supplier.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (supplier: SupplierDto) => {
    if (!confirm(`Delete supplier "${supplier.name}"?`)) return;
    try {
      await purchaseService.deleteSupplier(supplier.id);
      showToast('success', 'Deleted', 'Supplier removed.');
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not delete supplier.');
    }
  };

  const columns: Column<SupplierDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-slate-500' },
    { header: 'Name', accessor: 'name', sortKey: 'name', className: 'font-medium text-slate-100' },
    { header: 'Contact', accessor: (row) => row.contactName || '—', className: 'text-slate-400' },
    {
      header: 'Email / Phone',
      accessor: (row) => (
        <span className="text-xs text-slate-400">
          {row.email || '—'}<br />{row.phone || ''}
        </span>
      ),
      sortKey: 'email',
    },
    { header: 'Address', accessor: (row) => row.address || '—', className: 'text-xs text-slate-400' },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions module={APP_MODULES.Supplier} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            Supplier Management <Building2 className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">Full CRUD for vendor contacts</p>
        </div>
        <PermissionGate module={APP_MODULES.Supplier} action="Write">
          <Button onClick={openCreate}><Plus className="w-4 h-4" /> Add Supplier</Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader><CardTitle>Suppliers ({list.totalCount})</CardTitle></CardHeader>
        <CardContent className="space-y-4 sm:space-y-5">
          <ListToolbar
            searchInput={list.searchInput}
            onSearchChange={list.handleSearchChange}
            onSearchCommit={list.handleSearchCommit}
            searchPlaceholder="Search suppliers..."
          />

          <Table
            columns={columns}
            data={list.items}
            keyExtractor={(row) => row.id}
            isLoading={list.isLoading}
            sort={list.sort}
            onSortChange={list.toggleSort}
            emptyMessage="No suppliers yet."
            footer={
              <Pagination
                page={list.page}
                pageSize={list.pageSize}
                totalCount={list.totalCount}
                onPageChange={list.setPage}
                onPageSizeChange={list.setPageSize}
              />
            }
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Supplier' : 'Add Supplier'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>Company Name</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Contact Person</Label>
              <Input value={contactName} onChange={(e) => setContactName(e.target.value)} />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Email</Label>
                <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Phone</Label>
                <Input value={phone} onChange={(e) => setPhone(e.target.value)} />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Address</Label>
              <Input value={address} onChange={(e) => setAddress(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>{editing ? 'Update' : 'Create'}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
