import React, { useCallback, useEffect, useState } from 'react';
import { UserCircle, Plus, History } from 'lucide-react';
import { customerService } from '@/services';
import { CustomerDto } from '@/dtos';
import { CustomerSaleHistoryDto } from '@/dtos/credit.dto';
import { useAuth } from '@/context/AuthContext';
import { useActiveStoreId } from '@/context/StoreContext';
import { useToast } from '@/context/ToastContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
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
import { Badge } from '@/components/ui/badge';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';

export const CustomersPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = useActiveStoreId();

  const list = usePagedList<CustomerDto>({
    fetchFn: useCallback((query) => customerService.getCustomersPaged({ ...query, storeId }), [storeId]),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  useEffect(() => {
    const reload = () => list.reload();
    window.addEventListener('stockdaddy:store-changed', reload);
    return () => window.removeEventListener('stockdaddy:store-changed', reload);
  }, [list]);

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<CustomerDto | null>(null);
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [address, setAddress] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [historyCustomer, setHistoryCustomer] = useState<CustomerDto | null>(null);
  const [historyItems, setHistoryItems] = useState<CustomerSaleHistoryDto[]>([]);
  const [historyLoading, setHistoryLoading] = useState(false);

  const openHistory = async (row: CustomerDto) => {
    setHistoryCustomer(row);
    setHistoryLoading(true);
    try {
      const result = await customerService.getSalesHistory(row.id, { page: 1, pageSize: 50, sortBy: 'createdat', sortDir: 'desc' });
      setHistoryItems(result.items);
    } catch {
      setHistoryItems([]);
    } finally {
      setHistoryLoading(false);
    }
  };

  const resetForm = () => {
    setName('');
    setPhone('');
    setEmail('');
    setAddress('');
    setEditing(null);
  };

  const openCreate = () => {
    resetForm();
    setDialogOpen(true);
  };

  const openEdit = (row: CustomerDto) => {
    setEditing(row);
    setName(row.name);
    setPhone(row.phone);
    setEmail(row.email);
    setAddress(row.address);
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    setIsSubmitting(true);
    try {
      if (editing) {
        await customerService.updateCustomer(editing.id, {
          name: name.trim(),
          phone: phone.trim(),
          email: email.trim(),
          address: address.trim(),
        });
        showToast('success', 'Updated', 'Customer updated.');
      } else {
        await customerService.createCustomer({
          tenantId,
          name: name.trim(),
          phone: phone.trim(),
          email: email.trim(),
          address: address.trim(),
        });
        showToast('success', 'Created', 'Customer created.');
      }
      setDialogOpen(false);
      resetForm();
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not save customer.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: CustomerDto) => {
    if (!confirm(`Delete customer "${row.name}"?`)) return;
    try {
      await customerService.deleteCustomer(row.id);
      showToast('success', 'Deleted', 'Customer removed.');
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not delete customer.');
    }
  };

  const columns: Column<CustomerDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-muted-foreground' },
    { header: 'Name', accessor: 'name', sortKey: 'name', className: 'font-medium text-foreground' },
    { header: 'Email', accessor: (row) => row.email || '—', sortKey: 'email', className: 'text-muted-foreground' },
    { header: 'Phone', accessor: (row) => row.phone || '—', className: 'text-muted-foreground' },
    { header: 'Address', accessor: (row) => row.address || '—', className: 'text-xs text-muted-foreground' },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex items-center gap-1">
          <Button type="button" variant="ghost" size="sm" onClick={() => openHistory(row)} title="Purchase history">
            <History className="w-4 h-4" />
          </Button>
          <CrudRowActions module={APP_MODULES.Customer} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
        </div>
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero">
        <h1 className="page-hero-title">
          Customers <UserCircle className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-muted-foreground mt-1">Customer directory with full purchase history from POS checkouts</p>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Customer Directory ({list.totalCount})</CardTitle>
          <PermissionGate module={APP_MODULES.Customer} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add Customer
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search customers..."
            emptyMessage="No customers yet."
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Customer' : 'Add Customer'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>Name</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Phone</Label>
              <Input value={phone} onChange={(e) => setPhone(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>Email</Label>
              <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>Address</Label>
              <Input value={address} onChange={(e) => setAddress(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editing ? 'Update' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={!!historyCustomer} onOpenChange={(open) => !open && setHistoryCustomer(null)}>
        <DialogContent className="max-w-2xl max-h-[85vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Purchase history — {historyCustomer?.name}</DialogTitle>
          </DialogHeader>
          {historyLoading ? (
            <p className="text-sm text-muted-foreground">Loading…</p>
          ) : historyItems.length === 0 ? (
            <p className="text-sm text-muted-foreground">No purchases yet for this customer.</p>
          ) : (
            <div className="space-y-4">
              {historyItems.map((sale) => (
                <div key={sale.id} className="rounded-xl border border-border p-4 space-y-2">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <span className="font-mono text-sm text-blue-400">Sale #{sale.id}</span>
                    <span className="text-xs text-muted-foreground">{new Date(sale.createdAt).toLocaleString()}</span>
                    <Badge variant="secondary">{sale.paymentMethod}</Badge>
                    <span className="font-semibold text-emerald-400">${sale.totalAmount.toFixed(2)}</span>
                  </div>
                  {sale.discountAmount > 0 ? (
                    <p className="text-xs text-amber-400">Discount: −${sale.discountAmount.toFixed(2)}</p>
                  ) : null}
                  <ul className="text-xs text-muted-foreground space-y-1">
                    {sale.items.map((item) => (
                      <li key={item.id}>
                        {item.productName} ({item.skuCode}) · {item.quantity} × ${item.unitPrice.toFixed(2)} = $
                        {item.totalPrice.toFixed(2)}
                      </li>
                    ))}
                  </ul>
                </div>
              ))}
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};
