import React, { useCallback, useState } from 'react';
import { Settings, Shield, Cpu, Key, Database, Globe, Store, Plus, Sun, Moon, Monitor } from 'lucide-react';
import { Card } from '../components/common/Card';
import { Badge } from '../components/common/Badge';
import { useAuth } from '../context/AuthContext';
import { useTheme } from '../context/ThemeContext';
import { useToast } from '../context/ToastContext';
import { tenantService } from '@/services';
import { StoreDto } from '@/dtos';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
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
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { cn } from '@/lib/utils';
import type { ThemePreference } from '@/lib/theme';

const THEME_OPTIONS: {
  value: ThemePreference;
  label: string;
  description: string;
  icon: React.ComponentType<{ className?: string }>;
}[] = [
  {
    value: 'light',
    label: 'Light',
    description: 'Bright theme for daytime use',
    icon: Sun,
  },
  {
    value: 'dark',
    label: 'Dark',
    description: 'Low-light theme for night use',
    icon: Moon,
  },
  {
    value: 'system',
    label: 'System',
    description: 'Follows your device light or dark setting',
    icon: Monitor,
  },
];

export const SettingsPage: React.FC = () => {
  const { user } = useAuth();
  const { theme, resolvedTheme, setTheme } = useTheme();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;

  const list = usePagedList<StoreDto>({
    fetchFn: useCallback((query) => tenantService.getStoresPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingStore, setEditingStore] = useState<StoreDto | null>(null);
  const [storeName, setStoreName] = useState('');
  const [storeLocation, setStoreLocation] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

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
      list.reload();
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
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not delete store.');
    }
  };

  const columns: Column<StoreDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-muted-foreground' },
    {
      header: 'Name',
      accessor: (row) => (
        <span className="flex items-center gap-2 font-medium text-foreground">
          <Store className="w-4 h-4 text-blue-400 shrink-0" />
          {row.name}
        </span>
      ),
      sortKey: 'name',
    },
    { header: 'Location', accessor: (row) => row.location || '—', className: 'text-muted-foreground' },
    { header: 'Tenant', accessor: (row) => `#${row.tenantId}`, className: 'text-xs text-muted-foreground' },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions module={APP_MODULES.Settings} onEdit={() => openEditStore(row)} onDelete={() => handleDeleteStore(row)} />
      ),
    },
  ];

  return (
    <div className="page-stack max-w-4xl">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            System & Security Settings <Settings className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-muted-foreground mt-1">
            Configure stores, authentication, and multi-tenant parameters
          </p>
        </div>
        <Badge variant="success" size="md">
          System Active
        </Badge>
      </div>

      <Card title="General" subtitle="Appearance and display preferences">
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
          {THEME_OPTIONS.map(({ value, label, description, icon: Icon }) => (
            <button
              key={value}
              type="button"
              onClick={() => setTheme(value)}
              className={cn(
                'flex flex-col items-start gap-2 p-4 rounded-xl border text-left transition-all',
                theme === value
                  ? 'border-primary bg-primary/10 ring-2 ring-primary/30'
                  : 'border-border bg-card hover:bg-muted/50'
              )}
            >
              <Icon className="w-5 h-5 text-primary" />
              <span className="font-semibold text-foreground">{label}</span>
              <span className="text-xs text-muted-foreground">{description}</span>
            </button>
          ))}
        </div>
        <p className="text-xs text-muted-foreground mt-4">
          Active appearance:{' '}
          <span className="font-semibold text-foreground capitalize">{resolvedTheme}</span>
          {theme === 'system' ? ' (from system)' : ''}
        </p>
      </Card>

      <Card title="Store Locations" subtitle="Full CRUD for tenant store branches">
        <div className="flex justify-end mb-4">
          <PermissionGate module={APP_MODULES.Settings} action="Write">
            <Button onClick={openCreateStore}>
              <Plus className="w-4 h-4" /> Add Store
            </Button>
          </PermissionGate>
        </div>

        <PagedDataTable
          columns={columns}
          list={list}
          keyExtractor={(row) => row.id}
          searchPlaceholder="Search stores..."
          emptyMessage="No stores configured."
        />
      </Card>

      <Card
        title="Authentication & Authorization Architecture"
        subtitle="Current standalone JWT setup and AWS Cognito transition readiness"
      >
        <div className="space-y-4 text-sm text-foreground/90">
          <div className="p-4 rounded-xl bg-card/80 border border-border flex items-start gap-3">
            <Shield className="w-5 h-5 text-blue-400 mt-0.5 shrink-0" />
            <div>
              <h4 className="font-bold text-foreground">JWT Token Security (Current)</h4>
              <p className="text-xs text-muted-foreground mt-1">
                Requests to <code>/api/*</code> include the Bearer token in HTTP{' '}
                <code>Authorization</code> header via Axios Interceptors in <code>api.client.ts</code>.
              </p>
            </div>
          </div>

          <div className="p-4 rounded-xl bg-card/80 border border-border flex items-start gap-3">
            <Cpu className="w-5 h-5 text-purple-400 mt-0.5 shrink-0" />
            <div>
              <h4 className="font-bold text-foreground">AWS Cognito Migration Readiness</h4>
              <p className="text-xs text-muted-foreground mt-1">
                The frontend service layer is decoupled via <code>AuthContext</code> and{' '}
                <code>auth.service.ts</code>. Switching to AWS Cognito user pools only requires updating
                the <code>api.client.ts</code> token provider without refactoring component logic.
              </p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-2">
            <div className="p-3 rounded-xl bg-background border border-border">
              <span className="text-xs text-muted-foreground font-medium">Logged-in Username:</span>
              <p className="font-bold text-blue-400">{user?.username || 'N/A'}</p>
            </div>
            <div className="p-3 rounded-xl bg-background border border-border">
              <span className="text-xs text-muted-foreground font-medium">Current Tenant ID:</span>
              <p className="font-bold text-emerald-400">Tenant #{user?.tenantId || 1}</p>
            </div>
          </div>
        </div>
      </Card>

      <Card title="API Connection Parameters">
        <div className="space-y-3 text-xs">
          <div className="flex justify-between items-center py-2 border-b border-border">
            <span className="text-muted-foreground flex items-center gap-1.5">
              <Globe className="w-4 h-4 text-muted-foreground" /> Backend API Proxy:
            </span>
            <code className="px-2 py-1 rounded bg-card border border-border text-blue-400">
              http://localhost:5215/api
            </code>
          </div>

          <div className="flex justify-between items-center py-2 border-b border-border">
            <span className="text-muted-foreground flex items-center gap-1.5">
              <Database className="w-4 h-4 text-muted-foreground" /> PostgreSQL Persistence:
            </span>
            <span className="font-semibold text-foreground">Localhost SD DB</span>
          </div>

          <div className="flex justify-between items-center py-2">
            <span className="text-muted-foreground flex items-center gap-1.5">
              <Key className="w-4 h-4 text-muted-foreground" /> Security Scheme:
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
