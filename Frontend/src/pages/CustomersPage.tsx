import React, { useEffect, useState } from 'react';
import { UserCircle, Plus } from 'lucide-react';
import { customerService } from '@/services';
import { CustomerDto } from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
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
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';

export const CustomersPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;

  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<CustomerDto | null>(null);
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [address, setAddress] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadCustomers = async () => {
    setIsLoading(true);
    try {
      setCustomers(await customerService.getCustomers());
    } catch {
      showToast('error', 'Error', 'Failed to load customers.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadCustomers();
  }, []);

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
      loadCustomers();
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
      loadCustomers();
    } catch {
      showToast('error', 'Failed', 'Could not delete customer.');
    }
  };

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800">
        <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
          Customers <UserCircle className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-slate-400 mt-1">Manage customer records for sales and invoicing</p>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Customer Directory ({customers.length})</CardTitle>
          <PermissionGate module={APP_MODULES.Customer} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add Customer
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading...</p>
          ) : customers.length === 0 ? (
            <p className="text-sm text-slate-400">No customers yet.</p>
          ) : (
            <div className="space-y-2">
              {customers.map((row) => (
                <div
                  key={row.id}
                  className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                >
                  <div>
                    <p className="font-semibold text-slate-100">{row.name}</p>
                    <p className="text-xs text-slate-500">
                      {row.email || 'No email'} · {row.phone || 'No phone'}
                    </p>
                    {row.address && <p className="text-xs text-slate-500 mt-1">{row.address}</p>}
                  </div>
                  <CrudRowActions module={APP_MODULES.Customer} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
                </div>
              ))}
            </div>
          )}
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
    </div>
  );
};
