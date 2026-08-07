import React, { useEffect, useState } from 'react';
import { Settings, Shield, Cpu, Key, Database, Globe, Store, Plus } from 'lucide-react';
import { Card } from '../components/common/Card';
import { Badge } from '../components/common/Badge';
import { useAuth } from '../context/AuthContext';
import { useToast } from '../context/ToastContext';
import { tenantService } from '@/services';
import { StoreDto } from '@/dtos';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { CrudRowActions } from '@/components/common/CrudRowActions';

export const SettingsPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;

  const [stores, setStores] = useState<StoreDto[]>([]);
  const [isLoadingStores, setIsLoadingStores] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingStore, setEditingStore] = useState<StoreDto | null>(null);
  const [storeName, setStoreName] = useState('');
  const [storeLocation, setStoreLocation] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadStores = async () => {
    setIsLoadingStores(true);
    try {
      setStores(await tenantService.getStores());
    } catch {
      showToast('error', 'Error', 'Failed to load stores.');
    } finally {
      setIsLoadingStores(false);
    }
  };

  useEffect(() => {
    loadStores();
  }, []);

  const openCreateStore = () => {
    setEditingStore(null);
    setStoreName('');
    setStoreLocation('');
    setDialogOpen(true);
  };

  const openEditStore = (row: StoreDto) => {
    setEditingStore(row);
    setStoreName(row.name);
    setStoreLocation(row.location);
    setDialogOpen(true);
  };

  const handleSaveStore = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!storeName.trim()) return;
    setIsSubmitting(true);
    try {
      if (editingStore) {
        await tenantService.updateStore(editingStore.id, {
          name: storeName.trim(),
          location: storeLocation.trim(),
        });
        showToast('success', 'Updated', 'Store updated.');
      } else {
        await tenantService.createStore({
          tenantId,
          name: storeName.trim(),
          location: storeLocation.trim(),
        });
        showToast('success', 'Created', 'Store created.');
      }
      setDialogOpen(false);
      loadStores();
    } catch {
      showToast('error', 'Failed', 'Could not save store.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteStore = async (row: StoreDto) => {
    if (!confirm(`Delete store "${row.name}"?`)) return;
    try {
      await tenantService.deleteStore(row.id);
      showToast('success', 'Deleted', 'Store removed.');
      loadStores();
    } catch {
      showToast('error', 'Failed', 'Could not delete store.');
    }
  };

  return (
    <div className="space-y-6 max-w-4xl">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 glass-panel p-6 rounded-3xl border border-slate-800">
        <div>
          <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
            System & Security Settings <Settings className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Configure stores, authentication, and multi-tenant parameters
          </p>
        </div>
        <Badge variant="success" size="md">
          System Active
        </Badge>
      </div>

      <Card title="Store Locations" subtitle="Full CRUD for tenant store branches">
        <div className="flex justify-end mb-4">
          <Button onClick={openCreateStore}>
            <Plus className="w-4 h-4" /> Add Store
          </Button>
        </div>
        {isLoadingStores ? (
          <p className="text-sm text-slate-400">Loading stores...</p>
        ) : stores.length === 0 ? (
          <p className="text-sm text-slate-400">No stores configured.</p>
        ) : (
          <div className="space-y-2">
            {stores.map((row) => (
              <div
                key={row.id}
                className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
              >
                <div className="flex items-center gap-3">
                  <Store className="w-5 h-5 text-blue-400" />
                  <div>
                    <p className="font-semibold text-slate-100">{row.name}</p>
                    <p className="text-xs text-slate-500">
                      {row.location || 'No location'} · Tenant #{row.tenantId}
                    </p>
                  </div>
                </div>
                <CrudRowActions onEdit={() => openEditStore(row)} onDelete={() => handleDeleteStore(row)} />
              </div>
            ))}
          </div>
        )}
      </Card>

      <Card
        title="Authentication & Authorization Architecture"
        subtitle="Current standalone JWT setup and AWS Cognito transition readiness"
      >
        <div className="space-y-4 text-sm text-slate-300">
          <div className="p-4 rounded-xl bg-slate-900/80 border border-slate-800 flex items-start gap-3">
            <Shield className="w-5 h-5 text-blue-400 mt-0.5 shrink-0" />
            <div>
              <h4 className="font-bold text-slate-100">JWT Token Security (Current)</h4>
              <p className="text-xs text-slate-400 mt-1">
                Requests to <code>/api/*</code> include the Bearer token in HTTP{' '}
                <code>Authorization</code> header via Axios Interceptors in <code>api.client.ts</code>.
              </p>
            </div>
          </div>

          <div className="p-4 rounded-xl bg-slate-900/80 border border-slate-800 flex items-start gap-3">
            <Cpu className="w-5 h-5 text-purple-400 mt-0.5 shrink-0" />
            <div>
              <h4 className="font-bold text-slate-100">AWS Cognito Migration Readiness</h4>
              <p className="text-xs text-slate-400 mt-1">
                The frontend service layer is decoupled via <code>AuthContext</code> and{' '}
                <code>auth.service.ts</code>. Switching to AWS Cognito user pools only requires updating
                the <code>api.client.ts</code> token provider without refactoring component logic.
              </p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-2">
            <div className="p-3 rounded-xl bg-slate-950 border border-slate-800">
              <span className="text-xs text-slate-400 font-medium">Logged-in Username:</span>
              <p className="font-bold text-blue-400">{user?.username || 'N/A'}</p>
            </div>
            <div className="p-3 rounded-xl bg-slate-950 border border-slate-800">
              <span className="text-xs text-slate-400 font-medium">Current Tenant ID:</span>
              <p className="font-bold text-emerald-400">Tenant #{user?.tenantId || 1}</p>
            </div>
          </div>
        </div>
      </Card>

      <Card title="API Connection Parameters">
        <div className="space-y-3 text-xs">
          <div className="flex justify-between items-center py-2 border-b border-slate-800">
            <span className="text-slate-400 flex items-center gap-1.5">
              <Globe className="w-4 h-4 text-slate-500" /> Backend API Proxy:
            </span>
            <code className="px-2 py-1 rounded bg-slate-900 border border-slate-800 text-blue-400">
              http://localhost:5215/api
            </code>
          </div>

          <div className="flex justify-between items-center py-2 border-b border-slate-800">
            <span className="text-slate-400 flex items-center gap-1.5">
              <Database className="w-4 h-4 text-slate-500" /> PostgreSQL Persistence:
            </span>
            <span className="font-semibold text-slate-200">Localhost SD DB</span>
          </div>

          <div className="flex justify-between items-center py-2">
            <span className="text-slate-400 flex items-center gap-1.5">
              <Key className="w-4 h-4 text-slate-500" /> Security Scheme:
            </span>
            <span className="font-semibold text-emerald-400">HMAC-SHA256 Signed JWT</span>
          </div>
        </div>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingStore ? 'Edit Store' : 'Add Store'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSaveStore} className="space-y-4">
            <div className="space-y-2">
              <Label>Store Name</Label>
              <Input value={storeName} onChange={(e) => setStoreName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Location</Label>
              <Input value={storeLocation} onChange={(e) => setStoreLocation(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editingStore ? 'Update' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
